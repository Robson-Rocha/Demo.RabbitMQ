using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using data;
using EasyNetQ;

namespace produce
{
    class Program
    {
        private readonly static IBus _bus;

        private readonly static Timer _timerVerificacaoMudancas;

        private static DateTime _dataUltimaAlteracao;

        public static void VerificarMudancas(object sender, ElapsedEventArgs e)
        {
            _timerVerificacaoMudancas.Stop();
            try
            {
                var precosProdutosAlterados = new Dictionary<string, string>();
                var lojasNotificacao = new List<int>();

                using(var conn = new SqlConnection(Config.MSSQL_ConnectionString))
                {
                    conn.Open();
                    using(var cmd = new SqlCommand("select NomeProduto, ValorProduto from Produtos where DataUltimaAlteracao >= @dataUltimaAlteracao", conn))
                    {
                        cmd.Parameters.Add(new SqlParameter("dataUltimaAlteracao", _dataUltimaAlteracao));
                        using(var reader = cmd.ExecuteReader())
                        {
                            while(reader.Read())
                                precosProdutosAlterados.Add(reader.GetString(0), reader.GetString(1));
                        }
                    }

                    using(var cmd = new SqlCommand("select LojaId from Lojas", conn))
                    {
                        using(var reader = cmd.ExecuteReader())
                        {
                            while(reader.Read())
                                lojasNotificacao.Add(reader.GetInt32(0));
                        }
                    }
                }

                if(precosProdutosAlterados.Any() && lojasNotificacao.Any())
                {
                    Console.WriteLine($"{DateTime.Now} - Enviando notificação de alteração para as Lojas...");
                    Parallel.ForEach(lojasNotificacao, idLoja =>
                    {
                        var msg = new MensagemAlteracaoPrecosProdutos() {
                            PrecosProdutosAlterados = precosProdutosAlterados
                        };
                        _bus.SendAsync($"Loja.{idLoja}", msg );
                    });
                    Console.WriteLine($"{DateTime.Now} - Concluído.");
                    _dataUltimaAlteracao = DateTime.Now.AddHours(3);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            _timerVerificacaoMudancas.Start();
        }

        static void Main(string[] args)
        {
            Console.Title = "Producer";
            while (true)
            {
                Console.ReadLine();
            }
        }

        static Program()
        {
            _dataUltimaAlteracao = DateTime.Now.AddHours(3);
            _bus = RabbitHutch.CreateBus(Config.RabbitMQ_ConnectionString);
            _timerVerificacaoMudancas = new Timer();
            _timerVerificacaoMudancas.Elapsed += VerificarMudancas;
            _timerVerificacaoMudancas.Interval = Config.ProducerCheckInterval;
            _timerVerificacaoMudancas.Start();
        }
    }
}
