using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IcyBot
{
	/// <remarks/>
	[System.SerializableAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
	public partial class animeEntry
	{

		private int idField;

		private string titleField;

		private string englishField;

		private string synonymsField;

		private ushort episodesField;

		private decimal scoreField;

		private string typeField;

		private string statusField;

		private System.DateTime start_dateField;

		private System.DateTime end_dateField;

		private string synopsisField;

		private string imageField;

		/// <remarks/>
		public int id
		{
			get
			{
				return this.idField;
			}
			set
			{
				this.idField = value;
			}
		}

		/// <remarks/>
		public string title
		{
			get
			{
				return this.titleField;
			}
			set
			{
				this.titleField = value;
			}
		}

		/// <remarks/>
		public string english
		{
			get
			{
				return this.englishField;
			}
			set
			{
				this.englishField = value;
			}
		}

		/// <remarks/>
		public string synonyms
		{
			get
			{
				return this.synonymsField;
			}
			set
			{
				this.synonymsField = value;
			}
		}

		/// <remarks/>
		public ushort episodes
		{
			get
			{
				return this.episodesField;
			}
			set
			{
				this.episodesField = value;
			}
		}

		/// <remarks/>
		public decimal score
		{
			get
			{
				return this.scoreField;
			}
			set
			{
				this.scoreField = value;
			}
		}

		/// <remarks/>
		public string type
		{
			get
			{
				return this.typeField;
			}
			set
			{
				this.typeField = value;
			}
		}

		/// <remarks/>
		public string status
		{
			get
			{
				return this.statusField;
			}
			set
			{
				this.statusField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
		public System.DateTime start_date
		{
			get
			{
				return this.start_dateField;
			}
			set
			{
				this.start_dateField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
		public System.DateTime end_date
		{
			get
			{
				return this.end_dateField;
			}
			set
			{
				this.end_dateField = value;
			}
		}

		/// <remarks/>
		public string synopsis
		{
			get
			{
				return this.synopsisField;
			}
			set
			{
				this.synopsisField = value;
			}
		}

		/// <remarks/>
		public string image
		{
			get
			{
				return this.imageField;
			}
			set
			{
				this.imageField = value;
			}
		}
	}
}
