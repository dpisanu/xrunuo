using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests
{
	public class ShotAnArrowIntoTheAirQuest : BaseQuest
	{
		/*  I Shot an Arrow Into the Air... */
		public override object Title { get { return 1075486; } }

		/* Truth be told, the only way to get a feel for the bow is to shoot one and there's no better practice target than a 
		sheep. If ye can shoot ten of them I think ye will have proven yer abilities. Just grab a bow and make sure to take 
		enough ammunition. Bows tend to use arrows and crossbows use bolts. Ye can buy 'em or have someone craft 'em. How 
		about it then? Come back here when ye are done. */
		public override object Description { get { return 1075482; } }

		/* Fair enough, the bow isn't for everyone. Good day then. */
		public override object Refuse { get { return 1075483; } }

		/* Return once ye have killed ten sheep with a bow and not a moment before. */
		public override object Uncomplete { get { return 1075484; } }

		public ShotAnArrowIntoTheAirQuest()
			: base()
		{
			AddObjective( new SlayObjective( typeof( Sheep ), "sheep", 10 ) );

			AddReward( new BaseReward( typeof( TrinketBag ), 1072341 ) ); // A bag of trinkets.
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

	public class Kashiel : MondainQuester
	{
		public override Type[] Quests
		{
			get
			{
				return new Type[] 
			{ 
				typeof( ShotAnArrowIntoTheAirQuest )
			};
			}
		}

		[Constructable]
		public Kashiel()
			: base( "Kashiel", "the archer" )
		{
		}

		public Kashiel( Serial serial )
			: base( serial )
		{
		}

		public override void InitBody()
		{
			InitStats( 100, 100, 25 );

			Female = false;
			CantWalk = true;
			Race = Race.Human;

			Hue = 0x83FE;
			HairItemID = 0x2045;
			HairHue = 0x1;
		}

		public override void InitOutfit()
		{
			AddItem( new Backpack() );
			AddItem( new Boots( 0x1BB ) );

			Item item;

			item = new LeatherLegs();
			item.Hue = 0x901;
			AddItem( item );

			item = new LeatherGloves();
			item.Hue = 0x1BB;
			AddItem( item );

			item = new LeatherChest();
			item.Hue = 0x1BB;
			AddItem( item );

			item = new LeatherArms();
			item.Hue = 0x901;
			AddItem( item );

			item = new CompositeBow();
			item.Hue = 0x606;
			AddItem( item );
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