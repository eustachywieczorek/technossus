﻿using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Object = Java.Lang.Object;


namespace NavigationDrawer
{
	public class ProjectAdapter:BaseAdapter<Project>,IFilterable
	{
		List<Project> allproject;
		List<Project> partial;
		Activity activity;

		public ProjectAdapter(Activity a,IEnumerable<Project> project)
		{
			allproject = project.OrderBy(s => s.name).ToList();
			partial = null;
			activity = a;

			Filter = new ProjectFilter(this);
		}

		public override int Count {
			get {
				return allproject.Count;
			}
		}

		public override Object GetItem (int position)
		{
			return null; // could wrap a Contact in a Java.Lang.Object to return it here if needed
		}

		public override long GetItemId (int position)
		{
			return position;
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			var view = convertView ?? activity.LayoutInflater.Inflate (Resource.Layout.ProjectLayout, parent, false);
			var name = view.FindViewById<TextView> (Resource.Id.name);
			var ClientName = view.FindViewById<TextView> (Resource.Id.clientName);

			name.Text = allproject [position].name;
			ClientName.Text = "Client: "+ allproject [position].client;

			return view;

		}
			
		public override Project this[int position]
		{
			get { return allproject[position]; }
		}


		public Filter Filter { get; private set; }


		private class ProjectFilter : Filter
		{
			private readonly ProjectAdapter adapter;

			public ProjectFilter(ProjectAdapter adapter)
			{
				this.adapter = adapter;
			}

			protected override FilterResults PerformFiltering(ICharSequence constraint)
			{
				var returnObj = new FilterResults();
				var results = new List<Project>();

				if (adapter.partial == null)
					adapter.partial = adapter.allproject;

				if (constraint == null) return returnObj;

				if (adapter.partial != null && adapter.partial.Any())
				{
                    string lowerQuery = constraint.ToString().ToLower();
				
					results.AddRange(
						adapter.partial.Where(
                            project => QueryProject(project, lowerQuery)
                        ));
				}

				returnObj.Values = FromArray(results.Select(r => r.ToJavaObject()).ToArray());
				returnObj.Count = results.Count;
				constraint.Dispose();

				return returnObj;
			}

			protected override void PublishResults(ICharSequence constraint, FilterResults results)
			{
				using (var values = results.Values)				
					adapter.allproject = values.ToArray<Object>()
						.Select(r => r.ToNetObject<Project>()).ToList();
						
				adapter.NotifyDataSetChanged();
				constraint.Dispose();
				results.Dispose();
			}

            // Overall Query Method that takes in string, tokenizes it, and searches for projects that satisfy all terms
            private bool QueryProject(Project p, string query)
            {
                string[] tokens = query.Trim().Split(' ');
                foreach (string q in tokens)
                {
                    if (!QueryTokenProject(p, q))
                    {
                        return false;
                    }
                }

                return true;
            }

            // Query individual term in project
            private bool QueryTokenProject(Project p, string query)
            {
				if ((p.name != null && p.name.ToLower().Contains(query))
					|| (p.client != null && p.client.ToLower().Contains(query)) 
					|| (p.description != null && p.description.ToLower().Contains(query))
                    || DateUtil.isDuringMonth(p.startDate, query) 
                    || DateUtil.isDuringMonth(p.endDate,query))
                    return true;

                foreach (Employee e in p.teamMember)
                    if (e.name.ToLower().Contains(query))
                        return true;

                foreach (string s in p.technology)
                    if (s.ToLower().Contains(query))
                        return true;

                // If no cases match, default to false
                return false;
            }
		}


	}
}
