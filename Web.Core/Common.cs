using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Core
{
    public class Common
    {
        public static object GetValueByName<T>(T obj, string fieldName)
        {
            var t = obj.GetType();

            var prop = t.GetProperty(fieldName);

            return prop.GetValue(obj);
        }
        public static object GetValueByName(object obj, string fieldName)
        {
            var t = obj.GetType();

            var prop = t.GetProperty(fieldName);

            return prop.GetValue(obj);
        }

        /// <summary>
        /// Set Level cho danh mục
        /// </summary>
        private static readonly Dictionary<string, int> DicLevel = new Dictionary<string, int>();
        private static int _level;

        public static List<T> CreateLevel<T>(List<T> listAllCategory)
        {
            var lstParent = (from g in listAllCategory
                             where ((int)GetValueByName(g, "ParentID")) == 0
                             orderby (int)GetValueByName(g, "ThuTu")
                             select g).ToList<T>();
            var lstOrder = new List<T>();
            FindChild(listAllCategory, lstParent, ref lstOrder);
            return lstOrder;
        }

        public static void FindChild<T>(List<T> listAllCategory, List<T> lstParent, ref List<T> lstOrder)
        {
            using (var enumerator = lstParent.OrderBy(g => (int)GetValueByName(g, "ThuTu")).GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var item = enumerator.Current;
                    Func<KeyValuePair<string, int>, bool> predicate = g => g.Key == GetValueByName(item, "ParentID").ToString();
                    var pair = DicLevel.FirstOrDefault(predicate);
                    if (((int)GetValueByName(item, "ParentID")) == 0)
                    {
                        _level = 0;
                    }
                    if (string.IsNullOrEmpty(pair.Key))
                    {
                        DicLevel.Add(GetValueByName(item, "ParentID").ToString(), _level);
                    }
                    else
                    {
                        _level = pair.Value;
                    }
                    if (item != null)
                    {
                        var property = item.GetType().GetProperty("Level");
                        try
                        {
                            property.SetValue(item, _level, null);
                        }
                        catch (Exception)
                        {
                            break;
                        }
                    }
                    Func<T, bool> func2 = g => GetValueByName(g, "ID").ToString() == GetValueByName(item, "ID").ToString();
                    if (!lstOrder.Where(func2).Any())
                    {
                        lstOrder.Add(item);
                    }
                    Func<T, bool> func3 = g => GetValueByName(g, "ParentID").ToString() == GetValueByName(item, "ID").ToString();
                    var list = listAllCategory.Where<T>(func3).ToList<T>();
                    if (list.Count <= 0) continue;
                    foreach (var info2 in list.Select(local => item != null ? item.GetType().GetProperty("Level") : null))
                    {
                        try
                        {
                            info2.SetValue(item, _level, null);
                        }
                        catch (Exception)
                        {
                            break;
                        }
                    }
                    _level++;
                    FindChild(listAllCategory, list, ref lstOrder);
                }
            }
        }


        public static List<T> CreateLevelByLevel<T>(List<T> listAllCategory)
        {
            var lstParent = (from g in listAllCategory
                             where ((int)GetValueByName(g, "Level")) == 1
                             orderby (int)GetValueByName(g, "ThuTu")
                             select g).ToList<T>();
            var lstOrder = new List<T>();
            FindChildByLevel(listAllCategory, lstParent, ref lstOrder);
            return lstOrder;
        }

        public static void FindChildByLevel<T>(List<T> listAllCategory, List<T> lstParent, ref List<T> lstOrder)
        {
            using (var enumerator = lstParent.OrderBy(g => (int)GetValueByName(g, "ThuTu")).GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var item = enumerator.Current;
                    _level = (int)GetValueByName(item, "Level") == 1 ? 0 : (int)GetValueByName(item, "Level") - 1;
                    if (item != null)
                    {
                        var property = item.GetType().GetProperty("Level");
                        try
                        {
                            property.SetValue(item, _level, null);
                        }
                        catch (Exception)
                        {
                            break;
                        }
                    }
                    Func<T, bool> func2 = g => GetValueByName(g, "ID").ToString() == GetValueByName(item, "ID").ToString();
                    if (!lstOrder.Where(func2).Any())
                    {
                        lstOrder.Add(item);
                    }
                    Func<T, bool> func3 = g => GetValueByName(g, "ParentID").ToString() == GetValueByName(item, "ID").ToString();
                    var list = listAllCategory.Where<T>(func3).ToList<T>();
                    if (list.Count <= 0) continue;
                    foreach (var info2 in list.Select(local => item != null ? item.GetType().GetProperty("Level") : null))
                    {
                        try
                        {
                            info2.SetValue(item, _level, null);
                        }
                        catch (Exception)
                        {
                            break;
                        }
                    }
                    _level++;
                    FindChildByLevel(listAllCategory, list, ref lstOrder);
                }
            }
        }

        public static double RoudUp(double input, double number)
        {
            double rs = input / number;
            rs = Math.Round(rs, 0) * number;
            return rs;
        }

        /// <summary>
        /// Copy data từ object cũ sang object mới
        /// Áp dụng cho trường hợp Custom Model
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="objSource"></param>
        /// <param name="objTarget"></param>
        public static void CopyDataObj2Obj<T1, T2>(T1 objSource, T2 objTarget)
        {
            // Lấy danh sách properties từ object
            var lstpropSourceName = GetPropertiesNameOfClass(objSource);
            foreach (var propName in lstpropSourceName)
            {
                var val = GetValueByName(objSource, propName);
                var prop = objSource.GetType().GetProperty(propName);
                if (prop == null)
                {
                    continue;
                }
                try
                {
                    prop.SetValue(objTarget, val, null);
                }
                catch (Exception)
                {
                    continue;
                }
            }
        }
        /// <summary>
        /// Lấy list các thuộc tính trong object
        /// </summary>
        /// <param name="pObject"></param>
        /// <returns></returns>
        public static List<string> GetPropertiesNameOfClass<T>(T pObject)
        {
            var propertyList = new List<string>();
            if (pObject != null)
            {
                propertyList.AddRange(pObject.GetType().GetProperties().Select(prop => prop.Name));
            }
            return propertyList;
        }
        /// <summary>
        /// Delete file
        /// </summary>
        /// <param name="f">Đường dẫn file</param>
        /// <returns></returns>
        public static bool TryToDelete(string f)
        {
            try
            {
                // A.
                // Try to delete the file.
                File.Delete(f);
                return true;
            }
            catch (IOException)
            {
                // B.
                // We could not delete the file.
                return false;
            }
        }

        public static string Replace2Thumb(string featureImages)
        {
            if (!string.IsNullOrEmpty(featureImages))
            {
                return featureImages.Replace("/Images/Post/files", "/Images/Post/_thumbs/files");
            }
            //return "/Content/Site/images/noimage.png";
            return null;
        }
        private static List<int> lstChildID = new List<int>();

        public static List<int> GetChild<T>(List<T> lstCategory, int unitId, bool first = false, string ParentID = "ParentID", string vID = "ID", string Ordering = "Ordering")
        {
            if (!first)
                lstChildID = new List<int>();
            lstChildID.Add(unitId);
            var tmp = (from g in lstCategory
                       where ((int)GetValueByName(g, ParentID)) == unitId
                       orderby (int)GetValueByName(g, Ordering)
                       select g).ToList<T>();
            var id = 0;
            for (var i = 0; i < tmp.Count(); i++)
            {
                id = (int)GetValueByName(tmp[i], vID);
                lstChildID.Add(id);
                var tmpCount = (from g in lstCategory
                                where ((int)GetValueByName(g, ParentID)) == id
                                orderby (int)GetValueByName(g, Ordering)
                                select g).ToList<T>().Count();
                if (tmpCount > 0)
                    GetChild(lstCategory, id, true, ParentID, vID, Ordering);
            }
            return lstChildID;
        }
        private static List<int> lstParentID = new List<int>();
        public static List<int> GetAllParent<T>(List<T> lstCategory, int unitId, bool first = false, string ParentID = "ParentID", string vID = "ID")
        {
            if (!first)
                lstParentID = new List<int>();
            lstParentID.Add(unitId);
            var tmp = (from g in lstCategory
                       where ((int)GetValueByName(g, vID)) == unitId
                       select g).FirstOrDefault();
            int pid = (int)GetValueByName(tmp, ParentID);
            if (pid > 0)
            {
                GetAllParent(lstCategory, pid, true, ParentID, vID);
            }
            return lstParentID;
        }
    }
}
