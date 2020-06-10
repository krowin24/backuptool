using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MyApplication {

    public class Config {

        private Microsoft.Extensions.Configuration.IConfigurationBuilder m_Builder = null;

        public Config(string _configfile) {

            if (string.IsNullOrEmpty(_configfile))
                throw new ArgumentException("_configfile cannot be empty.");

            this.UpdateBasePath();
            this.FilePath = _configfile;

            if (!System.IO.File.Exists(_configfile))
                throw new System.IO.IOException("There is no configuration file.");

            this.m_Builder = new Microsoft.Extensions.Configuration.ConfigurationBuilder();
            this.m_Builder.AddJsonFile(this.FilePath);
        }

        ~Config() {}

        #region Methods

        public string GetValue(string section, string key) {

            // パラメーターチェック

            if (string.IsNullOrEmpty(section))
                throw new ArgumentException("section cannot be empty.");

            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("key cannot be empty.");

            Microsoft.Extensions.Configuration.IConfiguration Config = this.m_Builder.Build();

            // セクションの存在確認。
            if (!Microsoft.Extensions.Configuration.ConfigurationExtensions.Exists(Config.GetSection(section)))
                return null;

            return Config.GetSection(section)[key] ?? null;
        }

        private void UpdateBasePath() {
            this.BasePath = System.IO.Directory.GetParent(System.Reflection.Assembly.GetExecutingAssembly().Location).FullName;
        }

        #endregion

        #region Property

        /// <summary>
        /// Get the current directory as the base path of the configuration file.
        /// </summary>
        public string BasePath { private set; get; }

        /// <summary>
        /// Get the relative path to the configuration file.
        /// </summary>
        public string FilePath { private set; get; }

        #endregion

    }
}
