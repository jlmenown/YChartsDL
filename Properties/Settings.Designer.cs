﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace YChartsDL.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "12.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) " +
            "Chrome/83.0.4103.61 Safari/537.36")]
        public string httpUserAgent {
            get {
                return ((string)(this["httpUserAgent"]));
            }
            set {
                this["httpUserAgent"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("application/json")]
        public string httpContentType {
            get {
                return ((string)(this["httpContentType"]));
            }
            set {
                this["httpContentType"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("ycsessionid")]
        public string sessionCookieName {
            get {
                return ((string)(this["sessionCookieName"]));
            }
            set {
                this["sessionCookieName"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(".ycharts.com")]
        public string sessionCookieDomain {
            get {
                return ((string)(this["sessionCookieDomain"]));
            }
            set {
                this["sessionCookieDomain"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("https://ycharts.com/charts/fund_data.json?calcs=id:total_return_forward_adjusted_" +
            "price,include:true,,&format=real&securities=id:{0},include:true,,&splitType=sing" +
            "le&maxPoints=34000000")]
        public string ychartsUrlTemplate {
            get {
                return ((string)(this["ychartsUrlTemplate"]));
            }
            set {
                this["ychartsUrlTemplate"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("3000")]
        public int minDelayMillis {
            get {
                return ((int)(this["minDelayMillis"]));
            }
            set {
                this["minDelayMillis"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("5000")]
        public int maxDelayMillis {
            get {
                return ((int)(this["maxDelayMillis"]));
            }
            set {
                this["maxDelayMillis"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(",")]
        public string outputDelimiter {
            get {
                return ((string)(this["outputDelimiter"]));
            }
            set {
                this["outputDelimiter"] = value;
            }
        }
    }
}
