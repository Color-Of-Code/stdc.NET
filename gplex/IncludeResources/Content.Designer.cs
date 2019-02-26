namespace QUT.Gplex.IncludeResources {
    using System;
    using System.IO;
    using System.Reflection;

    /// <summary>
    /// var assembly = typeof(Program).GetTypeInfo().Assembly;
    /// string[] names = assembly.GetManifestResourceNames();
    ///
    /// gplex.Properties.Resources.resources
    /// gplex.SpecFiles.GplexBuffers.txt
    /// gplex.SpecFiles.ResourceHeader.txt
    /// gplex.SpecFiles.gplexx.frame
    /// gplex.SpecFiles.guesser.frame
    /// </summary>    
    internal class Content {
        
        internal Content() {
        }

        private static string GetString(string resourceName)
        {
            var assembly = typeof(Content).GetTypeInfo().Assembly;
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
        
        internal static string GplexBuffers {
            get {
                return GetString("gplex.SpecFiles.GplexBuffers.txt");
            }
        }
        
        internal static string GplexxFrame {
            get {
                return GetString("gplex.SpecFiles.gplexx.frame");
            }
        }
        
        internal static string ResourceHeader {
            get {
                return GetString("gplex.SpecFiles.ResourceHeader.txt");
            }
        }
    }
}
