using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Web;
using System.Web.Http;

namespace QL_ThuVien.Controllers
{
    /// <summary>
    /// Tỉnh, Thành phố
    /// </summary>
    [DataContract]
    public class Province
    {
        [DataMember]
        public string Code { get; set; }
        [DataMember]
        public string Name { get; set; }
    }
    /// <summary>
    /// Quận, Huyện
    /// </summary>
    [DataContract]
    public class District
    {
        [DataMember]
        public string Code { get; set; }
        [DataMember]
        public string Name { get; set; }
    }
    /// <summary>
    /// Phường, xã, thị trấn
    /// </summary>
    [DataContract]
    public class Ward
    {
        [DataMember]
        public string Code { get; set; }
        [DataMember]
        public string Name { get; set; }
    }
    public class ProvinceAPIController : ApiController
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public List<Province> Provinces = new List<Province>();
        private static string BaseUri;
        public ProvinceAPIController()
        {
            logger.Info($"IP {HttpContext.Current.Request.UserHostAddress}: Attached to API");
            BaseUri = "https://provinces.open-api.vn/";
        }
        public Province Get(int code)
        {
            return FindProvince(code.ToString());          
        }
        public IList<Province> Get(string name)
        {
            return FindProvinces(name);
        }
        public IList<Province> Get()
        {
            return GetAllProvinces();
        }
        /// <summary>
        /// Hàm gọi API
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="baseApi"></param>
        /// <param name="requestUri"></param>
        /// <returns></returns>
        private static IList<T> FetchAPI<T>(string baseApi, string requestUri)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseApi);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
                //GET Method
                var response = client.GetAsync(requestUri);
                response.Wait();
                var result = response.Result;
                if (result.IsSuccessStatusCode)
                {
                    var read = result.Content.ReadAsStringAsync();
                    read.Wait();
                    IList<T> provinces = JsonConvert.DeserializeObject<IList<T>>(read.Result);
                    return provinces;
                }
                else
                {
                    Console.WriteLine("Internal server Error");
                }
                return null;
            }
        }
        //Tỉnh, thành phố
        /// <summary>
        /// Lấy danh sách tỉnh, thành
        /// </summary>
        /// <returns></returns>
        public static IList<Province> GetAllProvinces()
        {
            return FetchAPI<Province>(BaseUri, "api/p");
        }
        /// <summary>
        /// Tìm tỉnh thành theo tên
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IList<Province> FindProvinces(string name)
        {
            return FetchAPI<Province>(BaseUri, "api/p/search/?q=" + name).Where(x => x.Name.Contains(name)).ToList();
        }
        /// <summary>
        /// Tìm tỉnh thành theo mã
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static Province FindProvince(string code)
        {
            return GetAllProvinces().FirstOrDefault(x => x.Code.Contains(code));
        }
        //Quận, huyện
        /// <summary>
        /// Lấy danh sách quận, huyện
        /// </summary>
        /// <returns></returns>
        public static IList<District> GetAllDistricts()
        {
            return FetchAPI<District>(BaseUri, "api/d");
        }
        /// <summary>
        /// Tìm quận, huyện theo tên
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IList<District> FindDistricts(string name)
        {
            return FetchAPI<District>(BaseUri, "api/d/search/?q=" + name).Where(x => x.Name.Contains(name)).ToList();
        }
        /// <summary>
        /// Tìm quận, huyện theo mã
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static District FindDistrict(string code)
        {
            return GetAllDistricts().FirstOrDefault(x => x.Code.Contains(code));
        }
        //Phường, xã, thị trấn
        /// <summary>
        /// Lấy danh sách phường, xã, thị trấn
        /// </summary>
        /// <returns></returns>
        public static IList<Ward> GetAllWards()
        {
            return FetchAPI<Ward>(BaseUri, "api/w");
        }
        /// <summary>
        /// Tìm phường, xã, thị trấn theo tên
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IList<Ward> FindWards(string name)
        {
            return FetchAPI<Ward>(BaseUri, "api/w/search/?q=" + name).Where(x => x.Name.Contains(name)).ToList();
        }
        /// <summary>
        /// Tìm phường, xã, thị trấn theo mã
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static Ward FindWard(string code)
        {
            return GetAllWards().FirstOrDefault(x => x.Code.Contains(code));
        }
    }
}
