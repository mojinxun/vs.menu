﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using VS.Menu.Model;
using VS.Menu.ThriftGenCore;

namespace VS.Menu.Helper
{
    /// <summary>
    /// 依赖的包的信息存储
    /// </summary>
    public class DependenceHelper
    {
        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="thriftFileName"></param>
        /// <returns></returns>
        public static List<DependenceModel> Get(string type)
        {
            var data = new List<DependenceModel>();
            var filePath = Path.Combine(Utility.AppBaseDic, "depend", type + ".data");
            if (string.IsNullOrEmpty(filePath))
                return data;

            try
            {
                if (File.Exists(filePath))
                    data = XmlHelper.XmlDeserializeFromFile<List<DependenceModel>>(filePath) ?? new List<DependenceModel>();
            }
            catch (Exception) { }

            if (data.Count <= 0)
            {
                if (type == ("thrift_" + EnumGenAsyncVersion.New))
                {
                    data = new List<DependenceModel>()
                    {
                        new DependenceModel(){ PackageId = "thrift_client", Version= "2.1.3", Namespace = "Thrift.Client" },
                    };
                }
                else if (type == ("thrift_" + EnumGenAsyncVersion.Old))
                {
                    data = new List<DependenceModel>()
                    {
                        new DependenceModel(){ PackageId = "thrift_client", Version= "1.0.0.1", Namespace = "Thrift.Client" },
                    };
                }
                else if (type == "grpc")
                {
                    data = new List<DependenceModel>()
                    {
                        new DependenceModel(){ PackageId = "Sodao.Core.Grpc", Version= "1.0.10.1", Namespace = "Sodao.Core.Grpc" },
                    };
                }
                Save(data, type);
            }

            return data;
        }

        /// <summary>
        /// 获取命名空间
        /// </summary>
        /// <returns></returns>
        public static string GetThriftNamespace()
        {
            var type = "thrift_" + ThriftGlobal.GenAsyncVersion;
            var dependencies = Get(type);
            return dependencies.FirstOrDefault(oo => oo.PackageId.ToLower().Contains("thrift"))?.Namespace ??
                "Thrift.Client";
        }

        /// <summary>
        /// 获取命名空间
        /// </summary>
        /// <returns></returns>
        public static string GetGrpcNamespace()
        {
            var type = "grpc";
            var dependencies = Get(type);
            return dependencies.FirstOrDefault(oo => oo.PackageId.ToLower().Contains("grpc"))?.Namespace ??
                "Sodao.Core.Grpc";
        }

        /// <summary>
        /// 保存thrift模板文件的文件夹路径
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public static void Save(List<DependenceModel> data, string type)
        {
            if (data == null || string.IsNullOrEmpty(type))
                return;

            var fold = Path.Combine(Utility.AppBaseDic, "depend");
            if (!Directory.Exists(fold))
                Directory.CreateDirectory(fold);

            var filePath = Path.Combine(fold, type + ".data");
            if (!File.Exists(filePath))
            {
                File.Create(filePath);
                Thread.Sleep(20);
            }

            XmlHelper.XmlSerializeToFile(data, filePath);
        }
    }
}