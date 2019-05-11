using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Timers;
using data;
using EasyNetQ;

namespace consume
{
    class Program
    {
        private static IBus bus;

        private static ConcurrentDictionary<string, string> ProdutosParaAtualizar;

        private static Timer TimerAtualizacaoProdutos;

        public static void ReceberMensagem(MensagemAlteracaoPrecosProdutos msg)
        {
            Console.WriteLine($"Recebidas solicitações para alterações nos preços de {msg.PrecosProdutosAlterados.Count} produtos:");
            foreach(var precoAlterado in msg.PrecosProdutosAlterados)
            {
                Console.WriteLine($"    Produto: {precoAlterado.Key}; Valor: {precoAlterado.Value}");
                AdicionarProdutosParaAtualizar(precoAlterado.Key, precoAlterado.Value);
            }
        }

        public static void AdicionarProdutosParaAtualizar(string nomeProduto, string novoPreco)
        {
            TimerAtualizacaoProdutos.Stop();
            ProdutosParaAtualizar.AddOrUpdate(nomeProduto, novoPreco, (k, v) => novoPreco);
            TimerAtualizacaoProdutos.Interval = Config.ConsumerBufferInterval;
            TimerAtualizacaoProdutos.Start();
        }

        public static void AtualizarProdutos(object sender, ElapsedEventArgs e)
        {
            TimerAtualizacaoProdutos.Stop();
            var produtos = ProdutosParaAtualizar.ToList();
            ProdutosParaAtualizar.Clear();

            foreach (var produto in produtos)
            {
                Console.WriteLine($"Atualizando o preço do produto '{produto.Key}' para '{produto.Value}'");
            }
        }

        static void Main(string[] args)
        {
            Console.Title = "Consumer";
            while (true)
            {
                Console.ReadLine();
            }
        }

        static Program()
        {
            ProdutosParaAtualizar = new ConcurrentDictionary<string, string>();
            TimerAtualizacaoProdutos = new Timer();
            TimerAtualizacaoProdutos.Elapsed += AtualizarProdutos;
            bus = RabbitHutch.CreateBus(Config.RabbitMQ_ConnectionString);
            bus.Receive($"Loja.{Config.ConsumerLojaId}", config =>
            {
                config.Add<MensagemAlteracaoPrecosProdutos>(msg => ReceberMensagem(msg));
            });
        }
    }
}
