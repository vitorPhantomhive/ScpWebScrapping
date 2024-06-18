using CsvHelper;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using WebScraping;

namespace StaticWebScraping
{
    public class Program
    {
        public static async void Main()
        {

            //URL da página alvo
            string url = "http://scp-pt-br.wikidot.com/scp-2176";
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string pageContent = await response.Content.ReadAsStringAsync();

                //carregar o conteúdo html na instancia de htmlDocument
                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(pageContent);

                //selecionando o nó html de interesse
                var pageContentNode = document.DocumentNode.SelectSingleNode("//*[@id='page-content']");

                if(pageContentNode == null)
                {
                    Console.WriteLine("Não foi possivel encontrar o nó page-content");
                    return;
                }

                //Inicializandoa lista de objetos que armazenará os dados raspados
                List<SCP> scpItems = new List<SCP>();

                var item = new SCP();

                var itemNumberNode = pageContentNode.SelectSingleNode(".//p[strong[contains(text(), Item nº)]]");
                if(itemNumberNode !=  null)
                {
                    item.ItemNumber = itemNumberNode.InnerText.Replace("Item nº", "").Trim();
                }

                Console.WriteLine(itemNumberNode);

            }
        }
    }

}
