using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests
{
	public class WatchYourStepQuest : BaseQuest
	{
		/* Watch Your Step */
		public override object Title { get { return 1074454; } }

		/* You won't be able to get into the sinkhole with any normal rope.  It's a big drop, sure, but the real problem is the acid 
		and poison gas.  See, most ropes can't stand up to that kind of treatment, even for a short period of time.  So you'll need 
		a special rope -- a magical rope. Like these ones I happen to have available here for a mere 5,000 gold each. */
		public override object Description { get { return 1074455; } }

		/* I sure don't want to go down there either.  You won't hear me chiding you. In fact, I commend your intellect. */
		public override object Refuse { get { return 1074456; } }

		/* The price isn't going to change.  It's 5,000 per rope please. */
		public override object Uncomplete { get { return 1074457; } }

		/* Be careful down there. I don't like losing customers. */
		public override object Complete { get { return 1074458; } }

		public WatchYourStepQuest()
			: base()
		{
			AddObjective( new ObtainObjective( typeof( Gold ), "gold", 5000, 0xEED ) );

			AddReward( new BaseReward( typeof( MagicalRope ), 1074338 ) ); // Magical Rope
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class Lucius : MondainQuester
	{
		public override Type[] Quests
		{
			get
			{
				return new Type[] 
			{ 
				typeof( WatchYourStepQuest )
			};
			}
		}

		[Constructable]
		public Lucius()
			: base( "Lucius", "the adventurer" )
		{
		}

		public Lucius( Serial serial )
			: base( serial )
		{
		}

		public override void InitBody()
		{
			InitStats( 100, 100, 25 );

			Female = false;
			CantWalk = true;
			Race = Race.Human;

			Hue = 0x83F3;
			HairItemID = 0x2047;
			HairHue = 0x393;
			FacialHairItemID = 0x203F;
			FacialHairHue = 0x393;
		}

		public override void InitOutfit()
		{
			AddItem( new Backpack() );
			AddItem( new Boots( 0x717 ) );
			AddItem( new LongPants( 0x1BB ) );
			AddItem( new Cloak( 0x71 ) );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}