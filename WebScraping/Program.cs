using HtmlAgilityPack;
using WebScraping;
using Newtonsoft.Json;
using CsvHelper;
using System.Globalization;
using System.Text.Json.Serialization;
using System;

namespace StaticWebScraping
{
    public class Program
    {
        public static async Task Main()
        {
           await Start();
           
        }

        public static async Task Start()
        { 
            //Inicializandoa lista de objetos que armazenará os dados raspados
            List<SCP> scpItems = new List<SCP>();
            for (int v = 99; v < 3000; v++)
            {
                //URL da página alvo
                string url = $"http://scp-pt-br.wikidot.com/scp-{v}";
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    //response.EnsureSuccessStatusCode();
                    string pageContent = await response.Content.ReadAsStringAsync();

                    //carregar o conteúdo html na instancia de htmlDocument
                    HtmlDocument document = new HtmlDocument();
                    document.LoadHtml(pageContent);

                    //selecionando o nó html de interesse
                    var pageContentNode = document.DocumentNode.SelectSingleNode("//*[@id='page-content']");

                   
                    var itemScp = new SCP();
                    //pegando os valores do html 
                    var itemNumberNode = pageContentNode.SelectSingleNode(".//p[strong[contains(text(),'Item nº')]]");

                    if (itemNumberNode == null)
                    {
                        Console.WriteLine($"Não foi possivel encontrar o scp: {v}");
                        continue;
                    }

                    if (itemNumberNode != null)
                    {
                        itemScp.ItemNumber = itemNumberNode.InnerText.Replace("Item nº", "").Trim();
                    }

                    var itemObjectClass = pageContentNode.SelectSingleNode(".//p[strong[contains(text(),'Classe do Objeto')]]");
                    if (itemObjectClass != null)
                    {
                        itemScp.ObjectClass = itemObjectClass.InnerText.Replace("Classe do Objeto", "");
                    }

                    // Selecionar o nó do cabeçalho "Procedimentos Especiais de Contenção"
                    var proceduresHeaderNode = pageContentNode.SelectSingleNode(".//p[strong[contains(text(),'Procedimentos Especiais de Contenção')]]");
                    if (proceduresHeaderNode != null)
                    {
                        //Criando uma lista de procedimentos
                        List<string> containmentProcedures = new List<string>();

                        //ADicionar o primeiro parágrafo (cabeçalho) sem o texto "Procedimentos de contenção"
                        containmentProcedures.Add(proceduresHeaderNode.InnerText.Replace("Procedimentos Especiais de Contenção", "").Trim());

                        //Selecionar todos os nós seguintes até encontrar outro cabeçalho ou fim do conteúdo
                        var nextSibling = proceduresHeaderNode.NextSibling;
                        while (nextSibling != null && nextSibling.Name != "strong")
                        {
                            // Verificar se o nó atual é um parágrafo e contém a tag <strong> com o texto "Descrição"
                            if (nextSibling.Name == "p" && nextSibling.SelectSingleNode(".//strong[contains(text(),'Descrição')]") != null)
                            {
                                break;
                            }
                            if (nextSibling.Name == "p" || nextSibling.Name == "#text")
                            {
                                containmentProcedures.Add((string)nextSibling.InnerText.Trim());
                            }
                            nextSibling = nextSibling.NextSibling;
                        }
                        // Combinar todos os parágrafos em uma única string
                        itemScp.ContainmentProcedures = string.Join("", containmentProcedures).Replace("\r\n", "");
                        //itemScp.ContainmentProcedures = proceduresHeaderNode.InnerText.Replace("Procedimentos Especiais de Contenção", "");
                    }

                    var itemDescription = pageContentNode.SelectSingleNode(".//p[strong[contains(text(),'Descrição')]]");
                    if (itemDescription != null)
                    {
                        List<string> descriptions = new List<string>();
                        itemScp.Description = itemDescription.InnerText.Replace("Descrição", "");

                        //selecionar todos os nós seguintes até encontrar outro cabeçalho ou acabar
                        var nextSibling = itemDescription.NextSibling;
                        while (nextSibling != null && nextSibling.Name != "div")
                        {
                            // Verificar se o nó atual é um parágrafo sem tag <strong> dentro dele
                            if (nextSibling.Name == "p" && nextSibling.SelectSingleNode(".//strong") == null)
                            {
                                descriptions.Add((string)nextSibling.InnerText.Trim());
                            }
                            else if (nextSibling.Name == "p" || nextSibling.SelectSingleNode(".//strong") != null)
                            {
                                break;
                            }
                            nextSibling = nextSibling.NextSibling;
                        }
                        itemScp.Description = string.Join("", descriptions).Replace("\r\n", "");
                    }
                    scpItems.Add(itemScp);

                }

                
                //indicando onde eu quero o caminho do arquivo
                string path = "C:\\Users\\vitor\\source\\repos\\WebScraping\\WebScraping\\json_scp\\json_scp.json";

                //trasnformando em json
                string json = JsonConvert.SerializeObject(scpItems);

                //criando o arquivo
                File.WriteAllText(path, json);

                Console.WriteLine($"Números de scps adicionados: " + scpItems.Count);
            }
            
        }
    }

}
