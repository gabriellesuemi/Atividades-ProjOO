using System;

public interface INotificacao
{
    void Enviar(string destino, string mensagem);
}

public enum TipoNotificacao
{
    Email,
    Sms,
    Push
}

public class EmailNotificacao : INotificacao
{
    public void Enviar(string destino, string mensagem)
    {
        Console.WriteLine($"(E-mail) Enviando para {destino}: {mensagem}");
    }
}

public class SmsNotificacao : INotificacao
{
    public void Enviar(string destino, string mensagem)
    {
        Console.WriteLine($"(SMS) Enviando para {destino}: {mensagem}");
    }
}

public class PushNotificacao : INotificacao
{
    public void Enviar(string destino, string mensagem)
    {
        Console.WriteLine($"(PUSH) Enviando para {destino}: {mensagem}");
    }
}

public static class NotificacaoFactory
{
    public static INotificacao CriarNotificacao(TipoNotificacao tipo)
    {
        switch (tipo)
        {
            case TipoNotificacao.Email:
                return new EmailNotificacao();
            case TipoNotificacao.Sms:
                return new SmsNotificacao();
            case TipoNotificacao.Push:
                return new PushNotificacao();
            default:
                throw new ArgumentException("Tipo de notificação inválido.");
        }
    }
}

public class ConfiguracaoGlobal
{
    private static ConfiguracaoGlobal instancia;

    public string NomeAplicacao { get; set; }
    public string ServidorEnvio { get; set; }
    public int MaxTentativasReenvio { get; set; }

    private ConfiguracaoGlobal()
    {
        NomeAplicacao = "Sistema de Notificações";
        ServidorEnvio = "sistemanotificacao.com";
        MaxTentativasReenvio = 3;
    }

    public static ConfiguracaoGlobal GetInstancia()
    {
        if (instancia == null)
        {
            instancia = new ConfiguracaoGlobal();
        }

        return instancia;
    }
}

public static class Testes
{
    public static void RodarTodos()
    {
        Console.WriteLine("TESTES");
        TestarFactoryEmail();
        TestarFactorySms();
        TestarFactoryPush();
        TestarSingleton();
        Console.WriteLine();
    }

    private static void TestarFactoryEmail()
    {
        INotificacao notificacao = NotificacaoFactory.CriarNotificacao(TipoNotificacao.Email);

        if (notificacao is EmailNotificacao)
            Console.WriteLine("Teste Factory Email: PASSOU");
        else
            Console.WriteLine("Teste Factory Email: FALHOU");
    }

    private static void TestarFactorySms()
    {
        INotificacao notificacao = NotificacaoFactory.CriarNotificacao(TipoNotificacao.Sms);

        if (notificacao is SmsNotificacao)
            Console.WriteLine("Teste Factory SMS: PASSOU");
        else
            Console.WriteLine("Teste Factory SMS: FALHOU");
    }

    private static void TestarFactoryPush()
    {
        INotificacao notificacao = NotificacaoFactory.CriarNotificacao(TipoNotificacao.Push);

        if (notificacao is PushNotificacao)
            Console.WriteLine("Teste Factory Push: PASSOU");
        else
            Console.WriteLine("Teste Factory Push: FALHOU");
    }

    private static void TestarSingleton()
    {
        ConfiguracaoGlobal c1 = ConfiguracaoGlobal.GetInstancia();
        ConfiguracaoGlobal c2 = ConfiguracaoGlobal.GetInstancia();

        if (object.ReferenceEquals(c1, c2))
            Console.WriteLine("Teste Singleton: PASSOU");
        else
            Console.WriteLine("Teste Singleton: FALHOU");
    }
}

class Program
{
    static void Main(string[] args)
    {
        Testes.RodarTodos();

        ConfiguracaoGlobal config = ConfiguracaoGlobal.GetInstancia();

        Console.WriteLine("CONFIGURAÇÃO GLOBAL");
        Console.WriteLine("Aplicação: " + config.NomeAplicacao);
        Console.WriteLine("Servidor de envio: " + config.ServidorEnvio);
        Console.WriteLine("Máximo de tentativas: " + config.MaxTentativasReenvio);
        Console.WriteLine();

        INotificacao notificacao1 = NotificacaoFactory.CriarNotificacao(TipoNotificacao.Email);
        notificacao1.Enviar("nome@email.com", "Bem-vindo ao sistema!");

        INotificacao notificacao2 = NotificacaoFactory.CriarNotificacao(TipoNotificacao.Sms);
        notificacao2.Enviar("12997206058", "Seu código é 8246.");

        INotificacao notificacao3 = NotificacaoFactory.CriarNotificacao(TipoNotificacao.Push);
        notificacao3.Enviar("nome", "Você recebeu uma nova mensagem.");
    }
}