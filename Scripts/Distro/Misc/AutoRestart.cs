using System;
using System.IO;
using Server.Commands;
using System.Diagnostics;
using Server;

namespace Server.Misc
{
	public class AutoRestart : Timer
	{
		public static bool Enabled = true; // is the script enabled?

		private static TimeSpan RestartTime = TimeSpan.FromHours( 5.0 ) + TimeSpan.FromMinutes( 55.0 ); // time of day at which to restart
		private static TimeSpan RestartDelay = TimeSpan.FromMinutes( 5.0 );

		private static TimeSpan WarningDelay = TimeSpan.FromMinutes( 1.0 ); // at what interval should the shutdown message be displayed?

		private static bool m_Restarting;
		private static DateTime m_RestartTime;

		private static int m_MessageCount = 5;

		public static bool Restarting { get { return m_Restarting; } }

		public static void Initialize()
		{
			CommandSystem.Register( "ToggleAutoRestart", AccessLevel.Developer, new CommandEventHandler( ToggleAutoRestart_OnCommand ) );
			CommandSystem.Register( "Restart", AccessLevel.Developer, new CommandEventHandler( Restart_OnCommand ) );
			new AutoRestart().Start();
		}

		[Usage( "ToggleAutoRestart <true | false>" )]
		[Description( "Determina si el servidor se reiniciara automaticamente a una hora establecida." )]
		public static void ToggleAutoRestart_OnCommand( CommandEventArgs e )
		{
			if ( Enabled )
			{
				Enabled = false;
				e.Mobile.SendMessage( "You have disabled the server auto restart." );
			}
			else
			{
				Enabled = true;

				if ( m_RestartTime < DateTime.UtcNow )
					m_RestartTime += TimeSpan.FromDays( 1.0 );

				e.Mobile.SendMessage( "You have enabled the server auto restart." );
			}
		}

		[Usage( "Restart" )]
		[Description( "Inicia una cuenta atras para reiniciar el servidor." )]
		public static void Restart_OnCommand( CommandEventArgs e )
		{
			if ( m_Restarting )
			{
				e.Mobile.SendMessage( "The server is already restarting." );
			}
			else
			{
				e.Mobile.SendMessage( "You have initiated server shutdown." );
				Enabled = true;
				m_RestartTime = DateTime.UtcNow;
			}
		}

		public AutoRestart()
			: base( TimeSpan.FromSeconds( 1.0 ), TimeSpan.FromSeconds( 1.0 ) )
		{

			m_RestartTime = DateTime.UtcNow.Date + RestartTime;

			if ( m_RestartTime < DateTime.UtcNow )
				m_RestartTime += TimeSpan.FromDays( 1.0 );
		}

		private void Warning_Callback()
		{
			if ( m_MessageCount > 0 )
			{
				World.Broadcast( 0x22, true, String.Format( "The server is restarting in {0} minute{1}.", m_MessageCount, m_MessageCount == 1 ? "" : "s" ) );
				m_MessageCount--;
			}
		}

		private void Restart_Callback()
		{
			World.Broadcast( 0x22, false, "The server is restarting..." );

			DailyBackup();

			World.Save();

			//Process.Start( Core.ExePath );

			Core.Process.Kill();
		}

		protected override void OnTick()
		{
			if ( m_Restarting || !Enabled )
				return;

			if ( DateTime.UtcNow < m_RestartTime )
				return;

			if ( WarningDelay > TimeSpan.Zero )
			{
				Warning_Callback();
				Timer.DelayCall( WarningDelay, WarningDelay, new TimerCallback( Warning_Callback ) );
			}

			m_Restarting = true;

			Timer.DelayCall( RestartDelay, new TimerCallback( Restart_Callback ) );
		}

		private static void DailyBackup()
		{
			try
			{
				string root = Path.Combine( Core.BaseDirectory, Path.Combine( "Backups", "Daily" ) );

				if ( !Directory.Exists( root ) )
					Directory.CreateDirectory( root );

				string saves = Path.Combine( Core.BaseDirectory, "Saves" );

				if ( Directory.Exists( saves ) )
					Directory.Move( saves, Path.Combine( root, String.Format( "Saves_{0}-{1}-{2}", DateTime.UtcNow.Day.ToString(), DateTime.UtcNow.Month.ToString(), DateTime.UtcNow.Year.ToString() ) ) );
			}
			catch
			{
			}
		}
	}
}
