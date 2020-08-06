using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using KeePass.Plugins;

namespace CreateDateTimeEntry
{
  [Serializable]
  public class CreateDateTimeEntrySettings
  {
    const string Name = "CreateDateTimeEntrySettings";

    static XmlSerializer Serializer;
    static CreateDateTimeEntrySettings()
    {
      Serializer = new XmlSerializer(typeof(CreateDateTimeEntrySettings));
    }

    private CreateDateTimeEntrySettings() { }

    public int YearIconIndex { get; set; } = 0;
    public int TimeIconIndex { get; set; } = 1;
    public int DayIconIndex { get; set; } = 2;
    public int MonthIconIndex { get; set; } = 3;

    private static string Export(CreateDateTimeEntrySettings data)
    {
      var sb = new StringBuilder();
      using (var writer = new StringWriter(sb))
      {
        var ns = new XmlSerializerNamespaces();
        ns.Add("", "");
        Serializer.Serialize(writer, data, ns);
      }
      return sb.ToString();
    }

    private static CreateDateTimeEntrySettings Import(string xml)
    {
      using (var reader = new StringReader(xml))
      {
        return Serializer.Deserialize(reader) as CreateDateTimeEntrySettings;
      }
    }

    public static CreateDateTimeEntrySettings Load(IPluginHost host)
    {
      var data = host.CustomConfig.GetString(Name, "");
      if (string.IsNullOrEmpty(data))
      {
        return new CreateDateTimeEntrySettings();
      }

      return Import(data);
    }

    public CreateDateTimeEntrySettings Clone()
    {
      return Import(Export(this));
    }

    public void Save(IPluginHost host)
    {
      var data = Export(this);
      host.CustomConfig.SetString(Name, data);
    }
  }
}
