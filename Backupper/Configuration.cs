
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApplication
{
    public class Configuration {

        #region Member

        private Microsoft.Extensions.Configuration.IConfiguration m_Root = null;

        private Microsoft.Extensions.Configuration.ConfigurationBuilder m_Builder = null;

        #endregion

        public Configuration() {}

        ~Configuration() { }

        #region Methods

        public bool LoadFromJsonFile(string configPath) {

            // パラメータチェック
            if (System.String.IsNullOrEmpty(configPath))
                return false;

            // ファイルの存在チェック
            if (!System.IO.File.Exists(configPath))
                return false;

            // ファイルの形式チェック
            string Extension = System.IO.Path.GetExtension(configPath);
            if (string.IsNullOrEmpty(Extension) || Extension != ".json")
                return false;
            
            this.Builder.AddJsonFile(configPath);
            this.m_Root = this.Builder.Build();

            return true;
        }

        public string GetValue(string sectionName, string key) {

            IConfigurationSection Section = this.GetSection(sectionName);

            if (Section is null) return null;
            if (string.IsNullOrEmpty(Section[key])) return null;

            return Section[key];
        }

        public IConfigurationSection GetSection(string sectionName) {

            if (this.m_Root is null) return null;

            IConfigurationSection Result = this.m_Root.GetSection(sectionName);

            if ((Result is null) || !Result.Exists())
                return null;
            
            return Result;
        }

        public IEnumerable<KeyValuePair<string, string>> GetSectionEnumerable(string sectionName) { return this.GetSection(sectionName).AsEnumerable() ?? null; }

        #endregion

        #region Property

        private Microsoft.Extensions.Configuration.ConfigurationBuilder Builder {
            get {
                if (this.m_Builder is null) this.m_Builder = new ConfigurationBuilder();
                return this.m_Builder;
            }
        }

        #endregion

    }
}
