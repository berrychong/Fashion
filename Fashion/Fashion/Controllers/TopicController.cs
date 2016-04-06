﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Fashion.Models;
using Fashion.Code.BLL;
namespace Fashion.Controllers
{
   
  
    public class TopicController : Controller
    {
        //
        // GET: /Topic/  
        public ActionResult Index()
        {
            
            return View();
        }
        public ActionResult Consult()
        {
            LoginStatusConfig();//配置登录状态
            if (Session["userName"] == null)
            {
                return Content("请先登录");
            }
            return View();
        }
        public ActionResult Test()
        {
            return View();
        }

        [HttpPost]
        public  ActionResult Test2( )
        {
            //上传头像
            var path = System.IO.Path.Combine(Server.MapPath("~/test"), Request.Files[0].FileName);
            Request.Files[0].SaveAs(path);
            return Content("成功");
        }
        
        
        
        public ActionResult Home()
        {
            LoginStatusConfig();//配置登录状态
            
            return View();
            
        }
        /// <summary>
        /// 获取Home页面的数据，帖子遍历的数据
        /// </summary>
        /// <returns></returns>
        public ActionResult AjaxHomeGetData()
        {
            //string theme = "{ 'name': 'dong' }";
            List<Dictionary<string,object>>list=new List<Dictionary<string,object>>();
            Dictionary<string,object>dic=new Dictionary<string,object>();
            dic.Add("name1","dong");
            //list.Add(dic);
            dic.Add("name2","xu");
            list.Add(dic);
            Dictionary<string, object> dic2 = new Dictionary<string, object>();
            dic2.Add("name3", "dong2");
            //list.Add(dic);
            dic2.Add("name4", "xu2");
            list.Add(dic2);
            JavaScriptSerializer serializer1 = new JavaScriptSerializer();
            string json = serializer1.Serialize(list);
            return Content(json);
            //return Content(theme);
        }

        
        
        public ActionResult Answer()
        {
            //ViewData["a"]=Request["type"].ToString();
            //ViewData["a"]="a";
            return View();
        }
        public ActionResult AjaxAnswer()
        {
            return Content("HelloWorld This is Answer");
        }
        public ActionResult Post()
        {
            LoginStatusConfig();//验证登录
            if (Session["userName"] == null)
            {
                return Content("请先登录");
            }
            return View();
        }
        public ActionResult fabu()
        {
            return View();
        }

        /// <summary>
        /// 提交数据
        /// 分为3个步骤：
        /// 1.将前端传回来的content保存为静态html
        /// 2.将帖子标题，内容的前200个字符保存到数据库
        /// 3.将帖子的图片的路径保存到数据库
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult postData()
        {
            ////先把前端传回来的content内容保存为静态页面
            //定义一个字节数组保存前端传回来的Post数据
            byte[]byteData=new byte[Request.InputStream.Length];
            //将流读取到byteData，InputStream读取到的是http头里的主体数据
            Request.InputStream.Read(byteData,0,byteData.Length);
            string postData = System.Text.Encoding.Default.GetString(byteData);
            postData = Server.UrlDecode(postData);
            //从postData提取出前端传回来的评论内容，即变量名为content的数据
            string[] datas = postData.Split('&');
            string contentData = datas[1].ToString();
            //去除变量名，如content=aaa，只取出aaa
            contentData = contentData.Substring(contentData.IndexOf('=')+1);
            
            //定义文件名fileName
            DateTime datetime = DateTime.Now;  
            string fileName = datetime.ToString("yyyyMMddHHmmss_ffff") + ".html";
            string fileNamePath = Server.MapPath("~/StaticHtml/TieZiHtml/") + fileName;
            //先判断文件是否存在，若存在：更换文件名,最后该文件名html文件
            while(System.IO.File.Exists(fileNamePath))
            {
                fileNamePath=Server.MapPath("~/StaticHtml/TieZiHtml/") + fileName;
            }
            System.IO.FileStream fs = new System.IO.FileStream(fileNamePath,System.IO.FileMode.Create);
            byte[] contentBytes = System.Text.Encoding.Default.GetBytes(contentData);
            fs.Write(contentBytes,0,contentBytes.Length);
            fs.Close();
            ///////保存静态html成功

            //将帖子数据保存到数据库
            Post_bll Post = new Post_bll();
            Theme_bll themeName = new Theme_bll();
            People_bll User = new People_bll();
            string caption = Request["question"].ToString();
            string userName = Session["userName"].ToString();
            int userId = User.GainUserId(userName);
            string theme = Request["theme"].ToString();
            int themeId = themeName.CollocateThemeId(theme);
            string staticHtmlPath = "~/StaticHtml/TieZiHtml/" + fileName;
            string content200 = datas[3].ToString();
            content200 = content200.Substring(content200.IndexOf('=') + 1);
       
            //过滤掉content200里图片
            System.Text.RegularExpressions.Regex regexImg = new System.Text.RegularExpressions.Regex(@"<img[^>]+>");
            content200 = regexImg.Replace(content200, "");
            //如果contentData的长度超过200,取contentData里的前两百个字符，将用于保存到数据库
            int len = content200.Length;
            if (len > 200)
            {
                len = 200;
            }
            content200 = content200.Substring(0, len);  
            //过滤掉所有的html标签
            //content200 = System.Text.RegularExpressions.Regex.Replace(content200, @"<[^>]*>", String.Empty);
            if (Post.finshInsert(caption, content200, userId, themeId, staticHtmlPath,datetime) != 1)
            {
                return Content("保存帖子信息时数据库出错");
            }
            //////获取所有图片里的图片路径,并且将图片路径保存到数据库里
            // 定义正则表达式用来匹配 img 标签
            System.Text.RegularExpressions.Regex regImg2 = new System.Text.RegularExpressions.Regex(@"<img\b[^<>]*?\bsrc[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.MatchCollection matches = regImg2.Matches(contentData);
            int i = 0;
            string[] strUrlList = new string[matches.Count];
            // 取得匹配项列表
            foreach (System.Text.RegularExpressions.Match match in matches)
            {
                strUrlList[i++] = match.Groups["imgUrl"].Value;
            }
            if(strUrlList.Length>=1)
            { 
            //根据帖子的标题查询数据库，得到该贴子的postId
            int postId = Post.GetPostId(caption);
            PostPhoto_bll postPhoto_bll = new PostPhoto_bll();
                
            if (postPhoto_bll.InsertPhotoUrl(postId, strUrlList) < 0)
            {
                return Content("保存图片路径时数据库出错");
            }
            }
            return Content("成功");
        }
        ///// <summary>
        ///// 验证登录是否成功；
        ///// 若登录失败，设置ViewData["LoginYes"] = 0；
        ///// 若登录成功，设置ViewData["LoginYes"] = 1；并且从数据库里取出用户头像的链接：ViewData["TouXiangUrl"] =...；
        ///// </summary>
        //public void CheckLogin()
        //{
        //    if (Session["userName"] == null)
        //    {//未登录
        //        ViewData["LoginYes"] = 0;

        //    }
        //    else
        //    {//已登录
        //        ViewData["LoginYes"] = 1;
        //        ViewData["userName"] = Session["userName"].ToString();
        //        People_bll peopleBll = new People_bll();
        //        ViewData["TouXiangUrl"] = peopleBll.GetImgUrlTouXiang(Session["userName"].ToString());//从数据库里获取头像url
        //    }
        //}






        /// <summary>
        /// 配置用户登录状态
        /// 如果已登录，返回true，并且设置登录状态：设置ViewData["LoginYes"] = 1；并且从数据库里取出用户头像的链接：ViewData["TouXiangUrl"] =...；
        /// 未登录返回false,并且设置ViewData["LoginYes"] = 0
        /// </summary>
        public bool LoginStatusConfig()
        {
            if (Session["userName"] == null)
            {//未登录
                ViewData["LoginYes"] = 0;
                return false;
            }
            //已登录
            ViewData["LoginYes"] = 1;
            ViewData["userName"] = Session["userName"].ToString();
            People_bll peopleBll = new People_bll();
            ViewData["TouXiangUrl"] = peopleBll.GetImgUrlTouXiang(Session["userName"].ToString());//从数据库里获取头像url
            return true;
        }

    }
}
