
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApplication.Archive {

    public class ArchiveEventArgs : System.EventArgs {

        public ArchiveEventArgs(string _source, string _destnation) : base() {

            if (System.String.IsNullOrEmpty(_source))
                throw new System.ArgumentException("_source is an invalid parameter.");

            if (System.String.IsNullOrEmpty(_destnation))
                throw new System.ArgumentException("_destnation is an invalid parameter.");

            this.Source = _source;
            this.Destnation = _destnation;

        }

        public string Source { private set; get; }
        
        public string Destnation { private set; get; }

    }
}
