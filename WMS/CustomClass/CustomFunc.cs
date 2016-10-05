using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web;

namespace WMS.CustomClass
{
    public class CustomFunc
    {
        public bool IsAllLetters(string s)
        {
            //bool result = name.All(x => char.IsLetter(x) || x == ' ' || x == '.' || x == ',');
            bool result = s.All(x => char.IsLetter(x) || x == ' ');

            //foreach (char c in s)
            //{
            //    if (!Char.IsLetter(c))
            //        return false;
            //}
            return result;
        }


        
       
    }
    public class CustomFunction
    {

        internal static List<Models.Company> GetCompanies(List<Models.Company> list, Models.User LoggedInUser)
        {
            switch (LoggedInUser.RoleID)
            {
                case 1: 
                    break;
                case 2:
                    list = list.Where(aa=>aa.CompID==1 || aa.CompID==2).ToList();
                    break;
                case 3:
                  list=  list.Where(aa => aa.CompID>=3).ToList();
                    break;
                case 4:
                  list=  list.Where(aa => aa.CompID ==LoggedInUser.CompanyID).ToList();
                    break;
                case 5:
                    break;
            }
            return list;
        }

        internal static System.Collections.IEnumerable GetLocations(List<Models.Location> locations, List<Models.UserLocation> userLocations)
        {
            List<Models.Location> tempLocs = new List<Models.Location>();
            foreach (var item in userLocations)
            {
                tempLocs.AddRange(locations.Where(aa => aa.LocID == item.LocationID).ToList());
            }

            return tempLocs;
        }
    }
}