using HtmlAgilityPack;
using WebScraping;

namespace StaticWebScraping
{
    public class Program
    {
        public static async Task Main()
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

                var itemScp = new SCP();
                //pegando os valores do html 
                var itemNumberNode = pageContentNode.SelectSingleNode(".//p[strong[contains(text(),'Item nº')]]");
                if(itemNumberNode !=  null)
                {
                    itemScp.ItemNumber = itemNumberNode.InnerText.Replace("Item nº", "").Trim();
                }
                var itemObjectClass = pageContentNode.SelectSingleNode(".//p[strong[contains(text(),'Classe do Objeto')]]");
                if(itemObjectClass != null)
                {
                    itemScp.ObjectClass = itemObjectClass.InnerText.Replace("Classe do Objeto", "");
                }
                // Selecionar o nó do cabeçalho "Procedimentos Especiais de Contenção"
                var proceduresHeaderNode = pageContentNode.SelectSingleNode(".//p[strong[contains(text(), 'Procedimentos Especiais de Contenção:')]]");

                if (proceduresHeaderNode != null)
                {
                    var currentNode = proceduresHeaderNode.NextSibling;
                    while (currentNode != null && !(currentNode.Name == "p" && currentNode.InnerText.Contains("Descrição:")))
                    {
                        if (currentNode.Name == "p")
                        {
                            itemScp.ContainmentProcedures += currentNode.InnerText + "\n";
                        }
                        currentNode = currentNode.NextSibling;
                    }
                }
                scpItems.Add(itemScp);
                foreach (var scp in scpItems)
                {
                    Console.WriteLine("Item Number: " + scp.ItemNumber);
                    Console.WriteLine("Classe do Objeto: " + scp.ObjectClass);
                    Console.WriteLine("Procedimentos Especiais de Contenção: " + scp.ContainmentProcedures);
                    // Você pode adicionar mais exibições aqui para ObjectClass, ContainmentProcedures e Description se necessário
                }

            }
        }
    }

}
