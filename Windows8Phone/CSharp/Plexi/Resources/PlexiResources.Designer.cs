﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34003
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Plexi.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class PlexiResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal PlexiResources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Plexi.Resources.PlexiResources", typeof(PlexiResources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to http://rez.stremor-apier.appspot.com/v1.
        /// </summary>
        internal static string Auditor {
            get {
                return ResourceManager.GetString("Auditor", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Stremor-Auth-Device.
        /// </summary>
        internal static string AuthDeviceHeader {
            get {
                return ResourceManager.GetString("AuthDeviceHeader", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to http://stremor-pud.appspot.com/auth.
        /// </summary>
        internal static string Authorization {
            get {
                return ResourceManager.GetString("Authorization", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Stremor-Auth-Token.
        /// </summary>
        internal static string AuthTokenHeader {
            get {
                return ResourceManager.GetString("AuthTokenHeader", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to http://casper-cached.stremor-nli.appspot.com/v1.
        /// </summary>
        internal static string Classifier {
            get {
                return ResourceManager.GetString("Classifier", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to http://stremor-pud.appspot/deauth.
        /// </summary>
        internal static string Deauthorization {
            get {
                return ResourceManager.GetString("Deauthorization", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to http://casper-cached.stremor-nli.appspot.com/v1/disambiguate.
        /// </summary>
        internal static string Disambiguator {
            get {
                return ResourceManager.GetString("Disambiguator", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to http://stremor-pud.appspot.com/v1/login.
        /// </summary>
        internal static string Login {
            get {
                return ResourceManager.GetString("Login", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to http://stremor-pud.appspot.com/v1/assist.
        /// </summary>
        internal static string Password {
            get {
                return ResourceManager.GetString("Password", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to http://stremor-pud.appspot.com/v1.
        /// </summary>
        internal static string Pud {
            get {
                return ResourceManager.GetString("Pud", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to http://stremor-pud.appspot.com/v1/signup.
        /// </summary>
        internal static string Registration {
            get {
                return ResourceManager.GetString("Registration", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to AuthToken.
        /// </summary>
        internal static string SettingsAuthKey {
            get {
                return ResourceManager.GetString("SettingsAuthKey", resourceCulture);
            }
        }
    }
}
