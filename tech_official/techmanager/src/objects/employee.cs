﻿using System;

namespace NavigationDrawer
{
	public class employee
	{
		public long id { get; set; }
		public string photo{ get; set; }
		public string name{ get; set; }
		public DateTime available{get;set;}
		public string technology{ get ; set; }

		public employee(long id, string photo,string name, string available,string technology)
		{
			this.id = id;
			this.photo = photo;
			this.name = name;
            this.available = DateUtil.convertToDateTime(available);
			this.technology = technology;
		}

	}
}

