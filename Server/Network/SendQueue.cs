using System;
using System.Collections;
using System.Collections.Generic;

namespace Server.Network
{
	public class SendQueue
	{
		private static readonly ILog log = LogManager.GetLogger( System.Reflection.MethodBase.GetCurrentMethod().DeclaringType );

		private class Entry
		{
			public byte[] m_Buffer;
			public int m_Length;

			private Entry( byte[] buffer, int length )
			{
				m_Buffer = buffer;
				m_Length = length;
			}

			private static readonly Stack<Entry> m_Pool = new Stack<Entry>();

			public static Entry Pool( byte[] buffer, int length )
			{
				lock ( m_Pool )
				{
					if ( m_Pool.Count == 0 )
						return new Entry( buffer, length );

					var e = m_Pool.Pop();

					e.m_Buffer = buffer;
					e.m_Length = length;

					return e;
				}
			}

			public static void Release( Entry e )
			{
				lock ( m_Pool )
				{
					m_Pool.Push( e );

					ReleaseBuffer( e.m_Buffer );
				}
			}
		}

		private static int m_CoalesceBufferSize = 512;
		private static BufferPool m_UnusedBuffers = new BufferPool( "Coalesced", 2048, m_CoalesceBufferSize );

		public static int CoalesceBufferSize
		{
			get { return m_CoalesceBufferSize; }
			set
			{
				if ( m_CoalesceBufferSize == value )
					return;

				if ( m_UnusedBuffers != null )
					m_UnusedBuffers.Free();

				m_CoalesceBufferSize = value;
				m_UnusedBuffers = new BufferPool( "Coalesced", 2048, m_CoalesceBufferSize );
			}
		}

		public static byte[] GetUnusedBuffer()
		{
			return m_UnusedBuffers.AcquireBuffer();
		}

		public static void ReleaseBuffer( byte[] buffer )
		{
			if ( buffer == null )
				log.Warning( "Attempting to release null packet buffer" );
			else if ( buffer.Length == m_CoalesceBufferSize )
				m_UnusedBuffers.ReleaseBuffer( buffer );
		}

		private readonly Queue m_Queue;
		private Entry m_Buffered;

		public bool IsFlushReady => ( m_Queue.Count == 0 && m_Buffered != null );

		public bool IsEmpty => ( m_Queue.Count == 0 && m_Buffered == null );

		public void Clear()
		{
			if ( m_Buffered != null )
			{
				Entry.Release( m_Buffered );
				m_Buffered = null;
			}

			while ( m_Queue.Count > 0 )
				Entry.Release( (Entry) m_Queue.Dequeue() );
		}

		public byte[] CheckFlushReady( ref int length )
		{
			var buffered = m_Buffered;

			if ( m_Queue.Count == 0 && buffered != null )
			{
				m_Buffered = null;

				m_Queue.Enqueue( buffered );
				length = buffered.m_Length;
				return buffered.m_Buffer;
			}

			return null;
		}

		public SendQueue()
		{
			m_Queue = new Queue();
		}

		public byte[] Peek( ref int length )
		{
			if ( m_Queue.Count > 0 )
			{
				var entry = (Entry) m_Queue.Peek();

				length = entry.m_Length;
				return entry.m_Buffer;
			}

			return null;
		}

		public byte[] Dequeue( ref int length )
		{
			Entry.Release( (Entry) m_Queue.Dequeue() );

			if ( m_Queue.Count > 0 )
			{
				var entry = (Entry) m_Queue.Peek();

				length = entry.m_Length;
				return entry.m_Buffer;
			}

			return null;
		}

		public bool Enqueue( byte[] buffer, int length )
		{
			if ( buffer == null )
			{
				log.Warning( "Attempting to send null packet buffer" );
				return false;
			}

			var space = 0;

			var success = false;
			while ( length > 0 )
			{
				if ( m_Buffered == null )
					m_Buffered = Entry.Pool( GetUnusedBuffer(), 0 );

				var tempbuffer = m_Buffered.m_Buffer;
				var differenceSpace = tempbuffer.Length - m_Buffered.m_Length;
				var availableSpace = ( length > differenceSpace ) ? differenceSpace : length;
				Buffer.BlockCopy( buffer, space, tempbuffer, m_Buffered.m_Length, availableSpace );
				m_Buffered.m_Length += availableSpace;
				space += availableSpace;
				length -= availableSpace;
				if ( m_Buffered.m_Length == tempbuffer.Length )
				{
					success = success || ( m_Queue.Count == 0 );
					m_Queue.Enqueue( m_Buffered );
					m_Buffered = null;
				}
			}
			return success;
		}
	}
}
