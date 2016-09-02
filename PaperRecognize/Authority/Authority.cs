using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using PaperRecognize.DTOs;
namespace PaperRecognize.Authority
{
    public class Authority
    {
        public static XmlDocument doc = new XmlDocument();

        public static void Open()
        {
            doc.Load(XmlReader.Create("Authority.xml"));
        }
        public static bool Access( UserRole role, string key )
        {
            XmlNode roleElement = null;
            XmlNodeList roles = doc.DocumentElement.ChildNodes;
            
            for (int i = 0; i < roles.Count; i++)
            {
                XmlAttributeCollection attrs = roles.Item(i).Attributes;
                for( int j = 0; j < attrs.Count; j++ )
                {
              
                    if(attrs.Item(j).Name.Equals("name", StringComparison.CurrentCultureIgnoreCase) 
                        && attrs.Item(j).Value.Equals(role.ToString(), StringComparison.CurrentCultureIgnoreCase))
                    {
                        roleElement = roles.Item(i);
                        break;
                    }
                }
                break;
            }
            if( null == roleElement )
            {
                return false;
            }
            XmlNodeList authorities = roleElement.ChildNodes;
            for (int i = 0; i < authorities.Count; i++)
            {
                if (authorities.Item(i).Name.Equals("authority-item"))
                {
                    XmlNode node = authorities.Item(i);
                    //node.chi
                }
            }
                return true;
        }

    }
}