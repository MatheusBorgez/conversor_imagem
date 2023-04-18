using ImageProcessor;
using ImageProcessor.Plugins.WebP.Imaging.Formats;

namespace conversor_imagem
{
    public class Program
    {
        static readonly List<string> tiposDeImagem = new()
        {
            ".bmp",
            ".gif",
            ".jpg",
            ".jpeg",
            ".png"
        };

        static int contador;
        static int maxValue;

        static void Main(string[] args)
        {
            try
            {
                var pathImagensRaiz = Config.PathPastaImagens;
                var diretorios = new List<string>(Directory.GetDirectories(pathImagensRaiz, "*", SearchOption.AllDirectories))
                {
                    pathImagensRaiz
                };

                Console.CursorVisible = false;
                contador = 0;
                maxValue = diretorios.SelectMany(c => Directory.GetFiles(c))
                                     .Where(e => tiposDeImagem.Contains(Path.GetExtension(e).ToLowerInvariant())).Count();

                maxValue *= 3;

                foreach (var diretorio in diretorios)
                {
                    var pathConvertidas = $"{diretorio}\\Converted";                    
                    ConverterImagensParaWebp(diretorio, pathConvertidas);
                }
            }
            catch (Exception ex)
            {
                WriteExceptionLog(ex);
            }
        }

        private static void ConverterImagensParaWebp(string pathImagens, string pathImagensConvertidas)
        {
            CrieDiretorioImagensConvertidas(pathImagensConvertidas);
            ConverterImagens(pathImagens, pathImagensConvertidas);
            ReplaceImagens(pathImagens, pathImagensConvertidas);
            RemoverImagensAntigas(pathImagens, pathImagensConvertidas);
        }

        private static void RemoverImagensAntigas(string pathImagens, string pathImagensConvertidas)
        {
            var imagensAntigas = Directory.GetFiles(pathImagens).Where(c => tiposDeImagem.Contains(Path.GetExtension(c).ToLowerInvariant()));
            foreach (var imagemAntiga in imagensAntigas)
            {
                File.Delete(imagemAntiga);

                contador++;
                double porcentagemCompleta = (double)contador / maxValue * 100;
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write("Progresso: {0}%", porcentagemCompleta.ToString("0.00"));
            }

            Directory.Delete(pathImagensConvertidas);
        }

        private static void ReplaceImagens(string pathImagens, string pathImagensConvertidas)
        {
            var imagensConvertidas = Directory.GetFiles(pathImagensConvertidas);
            var imagensAntigas = Directory.GetFiles(pathImagens).Where(c => tiposDeImagem.Contains(Path.GetExtension(c).ToLowerInvariant()));

            foreach (var imagemAntiga in imagensAntigas)
            {
                var nomeArquivo = Path.GetFileNameWithoutExtension(imagemAntiga);
                var imagemConvertida = imagensConvertidas
                    .FirstOrDefault(c => Path.GetFileNameWithoutExtension(c) == nomeArquivo);


                if (imagemConvertida != null)
                {
                    File.Move(imagemConvertida, Path.Combine(Path.GetDirectoryName(imagemAntiga), Path.GetFileName(imagemConvertida)), true);
                }

                contador++;
                double porcentagemCompleta = (double)contador / maxValue * 100;
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write("Progresso: {0}%", porcentagemCompleta.ToString("0.00"));
            }
        }

        private static void WriteExceptionLog(Exception ex)
        {
            var arquivoLog = Config.ArquivoLogExcecao;

            var logExcecao = new StreamWriter(arquivoLog);

            logExcecao.WriteLine(@$"{ex.Message}
                                    {ex.InnerException}
                                    {ex.StackTrace}");

            logExcecao.Close();
        }

        private static void CrieDiretorioImagensConvertidas(string pathImagensConvertidas)
        {
            if (!Directory.Exists(pathImagensConvertidas))
            {
                Directory.CreateDirectory(pathImagensConvertidas);
            }
        }

        private static void ConverterImagens(string pathImagens, string pathImagensConvertidas)
        {
            var imagensAntigas = Directory.GetFiles(pathImagens).Where(c => tiposDeImagem.Contains(Path.GetExtension(c).ToLowerInvariant()));

            foreach (var imagemParaConverter in imagensAntigas)
            {
                using var webPFileStream = new FileStream($"{pathImagens}\\{Path.GetFileNameWithoutExtension(imagemParaConverter)}.webp", FileMode.Create);
                using var imageFactory = new ImageFactory(preserveExifData: false);
                using var file = File.OpenRead(imagemParaConverter);

                imageFactory.Load(file)
                            .Format(new WebPFormat())
                            .Quality(80)
                            .Save(webPFileStream);

                contador++;
                double porcentagemCompleta = (double)contador / maxValue * 100;
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write("Progresso: {0}%", porcentagemCompleta.ToString("0.00"));
            }
        }

    }
}