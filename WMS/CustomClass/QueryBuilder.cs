using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using WMS.Models;

namespace WMS.CustomClass
{
    public class QueryBuilder
    {
        public DataTable GetValuesfromDB(string query)
        {
            DataTable dt = new DataTable();
            SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["TAS2013ConnectionString"].ConnectionString);

            using (SqlCommand cmdd = Conn.CreateCommand())
            using (SqlDataAdapter sda = new SqlDataAdapter(cmdd))
            {
                cmdd.CommandTimeout = 0;
                cmdd.CommandText = query;
                cmdd.CommandType = CommandType.Text;
                Conn.Open();
                sda.Fill(dt);
                Conn.Close();
            }
            return dt;
        }
        public string MakeCustomizeQuery(User _user)
        {
            string query = " where ";
            string subQuery = "";
            string subQueryLoc = "";
            List<string> _Criteria = new List<string>();
            List<string> _CriteriaForOr = new List<string>();
            List<string> _CriteriaForOrLoc = new List<string>();
           //if (_user.ViewLocation == true)
           // {
           //     _Criteria.Add(" LocID = " + _user.LocationID.ToString());
           // }
            TAS2013Entities db=  new TAS2013Entities();
            List<UserLocation> ulocs = new List<UserLocation>();
           ulocs = db.UserLocations.Where(aa => aa.UserID == _user.UserID).ToList();
            foreach (var uloc in ulocs)
            {
                _CriteriaForOrLoc.Add(" LocID = " + uloc.LocationID + " ");
            }
            if (_user.ViewContractual == true)
            {
                _CriteriaForOr.Add(" CatID = 3 ");
            }
            if (_user.ViewPermanentMgm == true)
            {
                _CriteriaForOr.Add(" CatID = 2  ");
            }
            if (_user.ViewPermanentStaff == true)
            {
                _CriteriaForOr.Add(" CatID = 4  ");
            }
            _CriteriaForOr.Add(" CatID=1 ");
            
            switch (_user.RoleID)
            {
                case 1:
                    break;
                case 2:
                    _Criteria.Add(" CompanyID= 1 or CompanyID = 2 ");
                    break;
                case 3:
                    _Criteria.Add(" CompanyID>= 3");
                    break;
                case 4:
                    _Criteria.Add(" CompanyID = "+_user.CompanyID.ToString());
                    break;
                case 5:
                    break;
            }
            for (int i = 0; i < _Criteria.Count; i++ )
            {
                query = query + _Criteria[i] + " and ";
            }
            for (int i = 0; i < _CriteriaForOrLoc.Count-1; i++)
            {
                subQueryLoc = subQueryLoc + _CriteriaForOrLoc[i] + " or ";
            }
            if(_CriteriaForOrLoc.Count !=0)
            subQueryLoc = " and  ( " + subQueryLoc + _CriteriaForOrLoc[_CriteriaForOrLoc.Count-1] + " ) ";
            //query = query + " ) and (";
            //query = query + _Criteria[_Criteria.Count-1];

            subQuery = " ( ";
            for (int i = 0; i < _CriteriaForOr.Count - 1; i++)
            {
                subQuery = subQuery + _CriteriaForOr[i] + " or ";
            }
            subQuery = subQuery + _CriteriaForOr[_CriteriaForOr.Count - 1];
            subQuery = subQuery + " ) ";
            query = query + subQuery + subQueryLoc;
            return query;
        }

        public string QueryForCompanySegeration(User _user)
        {
            string query = "";
            if (query != "")
            {
                query = " where " + query;
            }
            return query;
        }
        public string QueryForLocationSegeration(User _user)
        {
            TAS2013Entities db = new TAS2013Entities();
            List<UserLocation> ulocs = new List<UserLocation>();
            List<string> _CriteriaForOrLoc = new List<string>();
            ulocs = db.UserLocations.Where(aa => aa.UserID == _user.UserID).ToList();
            string query = " where ";
            foreach (var uloc in ulocs)
            {
                _CriteriaForOrLoc.Add(" LocationID = " + uloc.LocationID + " ");
            }
            for (int i = 0; i < _CriteriaForOrLoc.Count - 1; i++)
            {
                query = query + _CriteriaForOrLoc[i] + " or ";
            }
            query = query + _CriteriaForOrLoc[_CriteriaForOrLoc.Count - 1];
            return query;
        }
        public string QueryForLocationTableSegeration(User _user)
        {
            TAS2013Entities db = new TAS2013Entities();
            List<UserLocation> ulocs = new List<UserLocation>();
            List<string> _CriteriaForOrLoc = new List<string>();
            ulocs = db.UserLocations.Where(aa => aa.UserID == _user.UserID).ToList();
            string query = " where ";
            foreach (var uloc in ulocs)
            {
                _CriteriaForOrLoc.Add(" LocID = " + uloc.LocationID + " ");
            }
            for (int i = 0; i < _CriteriaForOrLoc.Count - 1; i++)
            {
                query = query + _CriteriaForOrLoc[i] + " or ";
            }
            query = query + _CriteriaForOrLoc[_CriteriaForOrLoc.Count - 1];
            return query;
        }
        public string QueryForLocationFilters(User _user)
        {
            TAS2013Entities db = new TAS2013Entities();
            List<UserLocation> ulocs = new List<UserLocation>();
            List<string> _CriteriaForOrLoc = new List<string>();
            ulocs = db.UserLocations.Where(aa => aa.UserID == _user.UserID).ToList();
            string query = "";
            foreach (var uloc in ulocs)
            {
                _CriteriaForOrLoc.Add(" LocID = " + uloc.LocationID + " ");
            }
            for (int i = 0; i < _CriteriaForOrLoc.Count - 1; i++)
            {
                query = query + _CriteriaForOrLoc[i] + " or ";
            }
            query = query + _CriteriaForOrLoc[_CriteriaForOrLoc.Count - 1];
            return query;
        }
        public string QueryForCompanyView(User _User)
        {
            string query = "";
            switch (_User.RoleID)
            {
                case 1:
                    break;
                case 2:
                    query = " where CompID= 1 or CompID = 2 ";
                    break;
                case 3:
                    query = " where  CompID>= 3";
                    break;
                case 4:
                    query = " where  CompID = " + _User.CompanyID.ToString();
                    break;
                case 5:
                    break;
            }
            return query;
        }
        public string QueryForCompanyFilters(User _User)
        {
            string query = "";
            switch (_User.RoleID)
            {
                case 1: 
                    break;
                case 2:
                    query = " where CompanyID= 1 or CompanyID = 2 ";
                    break;
                case 3:
                    query = " where  CompanyID>= 3";
                    break;
                case 4:
                    query = " where  CompanyID = " + _User.CompanyID.ToString();
                    break;
                case 5:
                    break;
            }
            return query;
        }

        public string QueryForCompanyViewLinq(User _User)
        {
            string query = "";
           switch (_User.RoleID)
            {
                case 1: query ="CompID > 0";
                    break;
                case 2:
                    query = "CompID= 1 or CompID = 2 ";
                    break;
                case 3:
                    query = "CompID>= 3";
                    break;
                case 4:
                    query = "CompID = " + _User.CompanyID.ToString();
                    break;
                case 5:
                    break;
            }
            return query;
        }

        public string QueryForCompanyViewForLinq(User _User)
        {
            string query = "";
            switch (_User.RoleID)
            {
                case 1: query = "CompanyID > 0";
                    break;
                case 2:
                    query = "CompanyID= 1 or CompanyID = 2 ";
                    break;
                case 3:
                    query = "CompanyID>= 3";
                    break;
                case 4:
                    query = "CompanyID = " + _User.CompanyID.ToString();
                    break;
                case 5:
                    break;
            }
            return query;
        }

        internal string QueryForLocationTableSegerationForLinq(User LoggedInUser)
        {
            TAS2013Entities db = new TAS2013Entities();
            List<UserLocation> ulocs = new List<UserLocation>();
            List<string> _CriteriaForOrLoc = new List<string>();
            ulocs = db.UserLocations.Where(aa => aa.UserID == LoggedInUser.UserID).ToList();
            String query = "";
            foreach (var uloc in ulocs)
            {
                _CriteriaForOrLoc.Add(" LocID = " + uloc.LocationID + " ");
            }
            for (int i = 0; i < _CriteriaForOrLoc.Count - 1; i++)
            {
                query = query + _CriteriaForOrLoc[i] + " or ";
            }
            if (_CriteriaForOrLoc.Count != 0)
                query = query + _CriteriaForOrLoc[_CriteriaForOrLoc.Count - 1];
            else
                query = "LocID > 0";
            return query;
        }

        internal string QueryForRegionFromCitiesForLinq(IEnumerable<City> cities)
        {

            String query = "";
            int d=1;
            foreach (var city in cities)
            {
              if(d <cities.Count())
                query = query + "RegionID="+city.RegionID+" or ";
            d++;
            }
            query = query + "RegionID=" + cities.Last().RegionID;
            return query;
        }

        internal string QueryForShiftForLinq(User LoggedInUser)
        {
            TAS2013Entities db = new TAS2013Entities();
            List<UserLocation> ulocs = new List<UserLocation>();
            List<string> _CriteriaForOrLoc = new List<string>();
            ulocs = db.UserLocations.Where(aa => aa.UserID == LoggedInUser.UserID).ToList();
            string query = "";
            foreach (var uloc in ulocs)
            {
                _CriteriaForOrLoc.Add(" LocationID = " + uloc.LocationID + " ");
            }
            for (int i = 0; i < _CriteriaForOrLoc.Count - 1; i++)
            {
                query = query + _CriteriaForOrLoc[i] + " or ";
            }
            query = query + _CriteriaForOrLoc[_CriteriaForOrLoc.Count - 1];
            return query;
        }
    }
}