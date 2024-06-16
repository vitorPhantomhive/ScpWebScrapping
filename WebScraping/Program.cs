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
        public static void Main()
        {
            try
            {
                // URL da página alvo da Wikipedia
                string url = "https://en.wikipedia.org/wiki/List_of_SpongeBob_SquarePants_episodes";

                var web = new HtmlWeb();
                // Baixando a página alvo e analisando seu conteúdo HTML
                var document = web.Load(url);

                // Selecionando os nós HTML de interesse
                var nodes = document.DocumentNode.SelectNodes("//*[@id=\"mw-content-text\"]/div[1]/table/tbody/tr");

                // Inicializando a lista de objetos que armazenará os dados raspados
                List<Episode> episodes = new List<Episode>();

                // Looping sobre os nós e extraindo dados deles
                foreach (var node in nodes)
                {
                    var overallNumberNode = node.SelectSingleNode("th[1]");
                    var titleNode = node.SelectSingleNode("td[2]");
                    var directorsNode = node.SelectSingleNode("td[3]");
                    var writtenByNode = node.SelectSingleNode("td[4]");
                    var releasedNode = node.SelectSingleNode("td[5]");

                    // Verifica se todos os nós necessários foram encontrados
                    if (overallNumberNode != null && titleNode != null && directorsNode != null && writtenByNode != null && releasedNode != null)
                    {
                        episodes.Add(new Episode()
                        {
                            OverallNumber = HtmlEntity.DeEntitize(overallNumberNode.InnerText.Trim()),
                            Title = HtmlEntity.DeEntitize(titleNode.InnerText.Trim()),
                            Directors = HtmlEntity.DeEntitize(directorsNode.InnerText.Trim()),
                            WrittenBy = HtmlEntity.DeEntitize(writtenByNode.InnerText.Trim()),
                            Released = HtmlEntity.DeEntitize(releasedNode.InnerText.Trim())
                        });
                    }
                }

                // Obtendo o diretório atual
                string currentDirectory = Directory.GetCurrentDirectory();
                Console.WriteLine("Diretório atual: " + currentDirectory);

                // Definindo o caminho do arquivo CSV
                string filePath = Path.Combine(currentDirectory, "output.csv");

                // Inicializando o arquivo CSV
                using (var writer = new StreamWriter(filePath))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    // Populando o arquivo CSV
                    csv.WriteRecords(episodes);
                }

                Console.WriteLine("Arquivo CSV gerado com sucesso em: " + filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ocorreu um erro: {ex.Message}");
            }
        }
    }

}
