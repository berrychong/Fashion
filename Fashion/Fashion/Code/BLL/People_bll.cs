﻿using Fashion.Code.DAL;
using Fashion.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Fashion.Code.BLL
{
    public class People_bll
    {
        /////////////////////////////////////////////
        //user表
        ////////////////////////////////////////////
        /// <summary>
        ///  user表
        /// 判断是否存在该用户
        /// 结果返回1代表存在
        /// 结果返回0代表不存在
        /// 结果返回2代表数据库出错，因为数据库存在超过2条数据的该用户
        /// </summary>
        /// <param name="userName"></param>
        public int HavingUserName(string userName)
        {
            
            User_dal user = new User_dal();
            int accountCount = (int)user.GetAccountCount(userName);
            if (accountCount<=0)
            {
                return 0;
            }
            if (accountCount > 1)
            {
                return 2;
            }
            else
            {
                return 1;
            }  
        }

       


        /// <summary>
        /// 判断登录是否成功，成功返回true，失败返回false
        /// 使用者：People控制器里的ajaxMakeLogin
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        public bool LoginYes(string userName,string password)
        {
            
            
            User_dal user_dal = new User_dal();
            /*object AccountCount = user_dal.GetAccountCount(userName);//用户的数量
            //null代表数据库不存在该数据，System.DBNull.Value代表数据库里存在数据，但是该字段的值为null
            if (AccountCount == null || AccountCount == System.DBNull.Value)
            {
                return false;
            }
            //如果用户的数量小于0
            if ((int)AccountCount <= 0)
            {
                return false;
            }
            if ((int)AccountCount > 1)
            {
                return false;
            }*/
            //以上判断存在该用户后，获取其盐值和密码
            User_model user_model = new User_model();
            user_model = user_dal.GetPwdAndSaltModel(userName);
            try
            {
                user_model = user_dal.GetPwdAndSaltModel(userName);
            }
            catch (Exception e)
            {
                //数据库异常处理，数据库里存在大于两条用户名一样的数据,抛出异常
                throw new Exception(e.ToString());
            }

            //finally { }
            
            string salt = user_model.salt; //颜值
            string realPassword = user_model.password; //密码
            //将盐值加在密码的后面，并转化为二进制
            byte[] pwdAndSaltBytes = System.Text.Encoding.UTF8.GetBytes(password + salt);
            //经过哈希算法加密后得到的二进制值
            byte[] hashBytes = new System.Security.Cryptography.SHA256Managed().ComputeHash(pwdAndSaltBytes);
            string hashPassword = Convert.ToBase64String(hashBytes);
            //判断密码是否正确
            if (realPassword == hashPassword)
            {
                return true;
            }
            else
            {
                return false;
            }      
        }

        /// <summary>
        /// 通过用户名获取该用户的头像的url，返回结果为string型
        /// 查询成功返回url的string格式
        /// null代表：查询到的数据为null，数据库中的User表里不存在该userName用户对应的数据
        ///           或查询到的该字段为null,即数据库存在该数据但数据库中的User表里用户名为userName的数据对应的User_TouXiangUrl为null
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <returns></returns>
        public string GetImgUrlTouXiang(string userName)
        {
            User_dal user = new User_dal();
            object UrlTouXiangObj = user.GetImgUrlTouXiang(userName);
            if (UrlTouXiangObj == null || UrlTouXiangObj == System.DBNull.Value)
            {
                return null;
            }
            return UrlTouXiangObj.ToString();
        }

        /// <summary>
        /// 通过用户名得到用户Id，返回用户Id
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public int GainUserId(string userName)
        {
            User_dal user = new User_dal();
            int userId = (int)user.GetUserId(userName);
            return userId;
        }

        
        /// <summary>
        /// 实现将用户的头像的url插入到数据库的功能，url为相对路径如：/Images/UserImages/TouXiang/userName.png
        /// 成功返回true 失败返回false
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="ImgExtension">图片扩展名</param>
        /// <returns></returns>
        public bool InsertUrlTouXiang(string userName, string ImgExtension)
        {
            string TouXiangUrl = "/Images/UserImages/TouXiang/" + userName + ImgExtension;
            User_dal user_dal = new User_dal();
            int NonqCount = user_dal.InsertUrlTouXiang(userName, TouXiangUrl); //受影响的函数
            if (NonqCount == 1)
            {
                return true;
            }
            else {
                return false;
            }
        }


        /// <summary>
        /// 实现将用户的全身照的url插入到数据库的功能，url为相对路径如：/Images/UserImages/QuanShenZhao/userName.png
        /// 成功返回true 失败返回false
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="ImgExtension">图片扩展名</param>
        /// <returns></returns>
        public bool InsertUrlQuanShenZhao(string userName, string ImgExtension)
        {
            string QuanShenZhaoUrl = "/Images/UserImages/TouXiang/" + userName + ImgExtension;
            User_dal user_dal = new User_dal();
            int NonqCount = user_dal.InsertUrlQuanShenZhao(userName, QuanShenZhaoUrl);//受影响的函数
            if (NonqCount == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }











        /// <summary>
        /// 实现专家注册功能，返回结果为int型
        /// 0代表注册成功；
        /// 1代表：数据插入出错，注册失败
        /// 2代表：查询到的数据为null，数据库中的等级表里不存在该rankName对应的数据
        /// 3代表：查询到的该字段为null,数据库中的等级表里等级名为rankName的数据对应的rankId为null
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="rankName">等级名</param>
        /// <param name="Email">电子邮箱</param>
        /// <param name="phoneNumber">手机号码</param>
        /// <param name="profession">职业</param>
        /// <param name="introduction">简介</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        public int ExpertRegister(string userName, string realName, string rankName, string Email, string phoneNumber, string profession, string introduction, string password)
        {
            //通过等级名得到等级编号
            Rank_dal rankDal = new Rank_dal();
            object rankIdObj = rankDal.GetRankId(rankName);
            if (rankIdObj == null)
            {
                return 2;
            }
            if (rankIdObj == System.DBNull.Value)
            {
                return 3;
            }
            string rankId = rankIdObj.ToString();

            //盐值
            string salt = Guid.NewGuid().ToString();
            //将盐值加在密码的后面，并转化为二进制
            byte[] pwdAndSaltBytes = System.Text.Encoding.UTF8.GetBytes(password + salt);
            //经过哈希算法加密后得到的二进制值
            byte[] hashBytes = new System.Security.Cryptography.SHA256Managed().ComputeHash(pwdAndSaltBytes);
            string hashPassword = Convert.ToBase64String(hashBytes);
            User_dal userDal = new User_dal();

            if (userDal.InsertExrertRegisterstring(userName, realName, hashPassword, salt, rankId, Email, phoneNumber, profession, introduction) == 1)
            {
                return 0;
            }
            else
            {
                return 1;
            }

        }
















        /////////////////////////////////////////////
        //user表和Rank表
        ////////////////////////////////////////////
        /// <summary>
        /// 实现注册功能，返回结果为int型
        /// 0代表注册成功；
        /// 1代表：数据插入出错，注册失败
        /// 2代表：查询到的数据为null，数据库中的等级表里不存在该rankName对应的数据
        /// 3代表：查询到的该字段为null,数据库中的等级表里等级名为rankName的数据对应的rankId为null
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="rankName">等级名（管理员，普通用户，时尚达人，专家）</param>
        /// <returns></returns>
        public int Register(string userName,string password,string rankName,string phoneNumberOrEmail)
        {
            //通过等级名得到等级编号
            Rank_dal rankDal = new Rank_dal();
            object rankIdObj = rankDal.GetRankId(rankName);
            if (rankIdObj == null)
            {
                return 2;
            }
            if (rankIdObj == System.DBNull.Value)
            {
                return 3;
            }
            string rankId = rankIdObj.ToString();

            //盐值
            string salt = Guid.NewGuid().ToString();
            //将盐值加在密码的后面，并转化为二进制
            byte[] pwdAndSaltBytes = System.Text.Encoding.UTF8.GetBytes(password+salt);
            //经过哈希算法加密后得到的二进制值
            byte[] hashBytes = new System.Security.Cryptography.SHA256Managed().ComputeHash(pwdAndSaltBytes);
            string hashPassword = Convert.ToBase64String(hashBytes);
            User_dal userDal = new User_dal();
            ////////////////////////////////////////////
            //通过正则表达式判断传进来的值是手机号还是邮箱
            //正则表达式字符串
            string emailStr =
            @"([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,5})+$";
            //邮箱正则表达式对象
            Regex emailReg = new Regex(emailStr);
            if (emailReg.IsMatch(phoneNumberOrEmail))
            {
                if (userDal.InsertEmailRegister(userName, salt, hashPassword, rankId, phoneNumberOrEmail) == 1)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
            else
            {
                if (userDal.InsertPhoneNumberRegister(userName, salt, hashPassword, rankId, phoneNumberOrEmail) == 1)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
            /////////////////////////////////////////////////
            
        }
        /// <summary>
        /// 更新个人信息的身高 腰围 体重 臀围 胸围 腿长 大腿围 小腿围 臂围 肤色 成功返回1
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="SkinColor"></param>
        /// <param name="Weight"></param>
        /// <param name="XiongWei"></param>
        /// <param name="YaoWei"></param>
        /// <param name="TunWei"></param>
        /// <param name="Height"></param>
        /// <param name="LegLength"></param>
        /// <param name="ThighGirth"></param>
        /// <param name="ArmGirth"></param>
        /// <param name="CalfGirth"></param>
        /// <returns></returns>
        public int UpdateBodyData(string UserName, string SkinColor, float Weight, float XiongWei, float YaoWei, float TunWei, float Height, float LegLength, float ThighGirth, float ArmGirth, float CalfGirth)
        {
            User_dal user = new User_dal();
            if (user.UpdateBodyData(UserName, SkinColor, Weight, XiongWei, YaoWei, TunWei, Height, LegLength, ThighGirth, ArmGirth, CalfGirth) == 1)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }



        /// <summary>
        /// 更新个人信息的数据 真实姓名，生日，职业，手机号，学历，兴趣
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="RealName"></param>
        /// <param name="BirthDate"></param>
        /// <param name="Profession"></param>
        /// <param name="PhoneNumber"></param>
        /// <param name="EducationalBackground"></param>
        /// <param name="Interest"></param>
        /// <returns></returns>
        public int UpdateInformation(string UserName, string RealName, string BirthDate, string Profession, string PhoneNumber, string EducationalBackground, string Interest)
        {
            User_dal user = new User_dal();

            if (user.UpdateInformation(UserName, RealName, BirthDate, Profession, PhoneNumber, EducationalBackground, Interest) == 1)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }


        /// <summary>
        /// 得到用户个人的肤色
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public string GetSkinColor(string userName)
        {
            User_dal user = new User_dal();
            DataTable dt = user.GetBodyData(userName);
            object skinColor= dt.Rows[0][0];
                 
            if (skinColor == null || skinColor == System.DBNull.Value)
            {
                return null;
            }
            else
            {
                return skinColor.ToString();
            }
        }
        /// <summary>
        /// 得到用户个人信息体重
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public float GetWeight(string userName)
        {
            User_dal user = new User_dal();
            DataTable dt = user.GetBodyData(userName);
            object Weight = dt.Rows[0][1];

            if (Weight == null || Weight == System.DBNull.Value)
            {
                return -1;
            }
            else
            {
                return float.Parse(Weight.ToString());
            }
        }
        /// <summary>
        /// 得到用户个人信息胸围
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public float GetXiongWei(string userName)
        {
            User_dal user = new User_dal();
            DataTable dt = user.GetBodyData(userName);
            object XiongWei = dt.Rows[0][2];

            if (XiongWei == null || XiongWei == System.DBNull.Value)
            {
                return -1;
            }
            else
            {
                return float.Parse(XiongWei.ToString());
            }
        }

        /// <summary>
        /// 得到用户个人信息腰围
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public float GetYaoWei(string userName)
        {
            User_dal user = new User_dal();
            DataTable dt = user.GetBodyData(userName);
            object YaoWei = dt.Rows[0][3];

            if (YaoWei == null || YaoWei == System.DBNull.Value)
            {
                return -1;
            }
            else
            {
                return float.Parse(YaoWei.ToString());
            }
        }


        /// <summary>
        /// 得到用户个人信息臀围
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public float GetTunWei(string userName)
        {
            User_dal user = new User_dal();
            DataTable dt = user.GetBodyData(userName);
            object TunWei = dt.Rows[0][4];

            if (TunWei == null || TunWei == System.DBNull.Value)
            {
                return -1;
            }
            else
            {
                return float.Parse(TunWei.ToString());
            }
        }


        /// <summary>
        /// 得到用户个人信息身高
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public float GetHeight(string userName)
        {
            User_dal user = new User_dal();
            DataTable dt = user.GetBodyData(userName);
            object Height = dt.Rows[0][5];

            if (Height == null || Height == System.DBNull.Value)
            {
                return -1;
            }
            else
            {
                return float.Parse(Height.ToString());
            }
        }

        /// <summary>
        /// 得到用户个人信息腿长
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public float GetLegLength(string userName)
        {
            User_dal user = new User_dal();
            DataTable dt = user.GetBodyData(userName);
            object LegLength = dt.Rows[0][6];

            if (LegLength == null || LegLength == System.DBNull.Value)
            {
                return -1;
            }
            else
            {
                return float.Parse(LegLength.ToString());
            }
        }


        /// <summary>
        /// 得到用户个人信息大腿围
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public float GetThighGirth(string userName)
        {
            User_dal user = new User_dal();
            DataTable dt = user.GetBodyData(userName);
            object ThighGirth = dt.Rows[0][7];

            if (ThighGirth == null || ThighGirth == System.DBNull.Value)
            {
                return -1;
            }
            else
            {
                return float.Parse(ThighGirth.ToString());
            }
        }

        /// <summary>
        /// 得到用户个人信息小腿围
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public float GetCalfGirth(string userName)
        {
            User_dal user = new User_dal();
            DataTable dt = user.GetBodyData(userName);
            object CalfGirth = dt.Rows[0][8];

            if (CalfGirth == null || CalfGirth == System.DBNull.Value)
            {
                return -1;
            }
            else
            {
                return float.Parse(CalfGirth.ToString());
            }
        }


        /// <summary>
        /// 得到用户个人信息手臂围
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public float GetArmGirth(string userName)
        {
            User_dal user = new User_dal();
            DataTable dt = user.GetBodyData(userName);
            object ArmGirth = dt.Rows[0][9];

            if (ArmGirth == null || ArmGirth == System.DBNull.Value)
            {
                return -1;
            }
            else
            {
                return float.Parse(ArmGirth.ToString());
            }
        }

        /// <summary>
        /// 检查数据库得到的真实姓名
        /// </summary>
        /// <param name="useName"></param>
        /// <returns></returns>
        public string CheckRealName(string userName)
        {
            User_dal user = new User_dal();
            DataTable dt = user.GetPersonalInformation(userName);
            object RealName = dt.Rows[0][0];
            
            if (RealName == null || RealName == System.DBNull.Value)
            {
                return null;
            }
            else
            {
                return RealName.ToString();
            }
        }
       /// <summary>
        /// 检查数据库得到的出生日期
       /// </summary>
       /// <param name="userName"></param>
       /// <returns></returns>
        public string CheckBirthDate(string userName)
        {
            User_dal user = new User_dal();
            DataTable dt = user.GetPersonalInformation(userName);
            object BirthDate = dt.Rows[0][1];
            if (BirthDate == null || BirthDate == System.DBNull.Value)
            {
                return null;
            }
            else 
            {
                return String.Format("{0:yyyy\\/MM\\/dd}", BirthDate);
            }
           
        }
        /// <summary>
        /// 检查数据库得到的职业
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public string CheckProfession(string userName)
        {
            User_dal user = new User_dal();
            DataTable dt = user.GetPersonalInformation(userName);
            object Profession = dt.Rows[0][2];
            if (Profession == null || Profession == System.DBNull.Value)
            {
                return null;
            }
            else
            {
                return Profession.ToString(); 
            }

        }
        /// <summary>
        ///  检查数据库得到的手机号
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public string CheckPhoneNumber(string userName)
        {
            User_dal user = new User_dal();
            DataTable dt = user.GetPersonalInformation(userName);
            object PhoneNumber = dt.Rows[0][3];
            if (PhoneNumber == null || PhoneNumber == System.DBNull.Value)
            {
                return null;
            }
            else
            {
                return PhoneNumber.ToString();
            }

        }
        /// <summary>
        /// 检查数据库得到的学历
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public string CheckEducationalBackground(string userName)
        {
            User_dal user = new User_dal();
            DataTable dt = user.GetPersonalInformation(userName);
            object EducationalBackground = dt.Rows[0][4];
            if (EducationalBackground == null || EducationalBackground == System.DBNull.Value)
            {
                return null;
            }
            else
            {
                return EducationalBackground.ToString();
            }

        }

        /// <summary>
        /// 检查数据库得到的兴趣
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public string CheckInterest(string userName)
        {
            User_dal user = new User_dal();
            DataTable dt = user.GetPersonalInformation(userName);
            object Interest = dt.Rows[0][5];
            if (Interest == null || Interest == System.DBNull.Value)
            {
                return null;
            }
            else
            {
                return Interest.ToString();
            }

        }



        

    }
}