using System;
using System.Collections.Generic;

public interface INotificacao
{
    void Enviar(string destino, string mensagem);
}

public enum TipoNotificacao
{
    Email,
    Sms,
    Push,
    SmsExterno
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

// Adapter
public class SmsExternoLegado
{
    public void SendSms(string numero, string texto)
    {
        Console.WriteLine($"(SMS EXTERNO / LEGADO) Enviando para {numero}: {texto}");
    }
}

public class SmsExternoAdapter : INotificacao
{
    private SmsExternoLegado smsExterno;

    public SmsExternoAdapter(SmsExternoLegado smsExterno)
    {
        this.smsExterno = smsExterno;
    }

    public void Enviar(string destino, string mensagem)
    {
        smsExterno.SendSms(destino, mensagem);
    }
}

// Proxy
public class NotificacaoProxy : INotificacao
{
    private INotificacao notificacaoReal;
    private static Dictionary<string, int> tentativasPorDestino = new Dictionary<string, int>();

    public NotificacaoProxy(INotificacao notificacaoReal)
    {
        this.notificacaoReal = notificacaoReal;
    }

    public void Enviar(string destino, string mensagem)
    {
        Console.WriteLine("[LOG] Iniciando processo de envio...");

        if (!ValidarPermissao(destino))
        {
            Console.WriteLine("[LOG] Envio bloqueado: destino sem permissão.");
            return;
        }

        if (!ValidarMensagem(mensagem))
        {
            Console.WriteLine("[LOG] Envio bloqueado: mensagem inválida.");
            return;
        }

        if (!VerificarLimiteTentativas(destino))
        {
            Console.WriteLine("[LOG] Envio bloqueado: limite de tentativas excedido.");
            return;
        }

        RegistrarTentativa(destino);

        notificacaoReal.Enviar(destino, mensagem);

        Console.WriteLine("[LOG] Envio realizado com sucesso.");
    }

    private bool ValidarPermissao(string destino)
    {
        return !string.IsNullOrWhiteSpace(destino);
    }

    private bool ValidarMensagem(string mensagem)
    {
        return !string.IsNullOrWhiteSpace(mensagem);
    }

    private bool VerificarLimiteTentativas(string destino)
    {
        ConfiguracaoGlobal config = ConfiguracaoGlobal.GetInstancia();

        if (!tentativasPorDestino.ContainsKey(destino))
            return true;

        return tentativasPorDestino[destino] < config.MaxTentativasReenvio;
    }

    private void RegistrarTentativa(string destino)
    {
        if (!tentativasPorDestino.ContainsKey(destino))
            tentativasPorDestino[destino] = 0;

        tentativasPorDestino[destino]++;
    }
}

public static class NotificacaoFactory
{
    public static INotificacao CriarNotificacao(TipoNotificacao tipo)
    {
        INotificacao notificacaoBase;

        switch (tipo)
        {
            case TipoNotificacao.Email:
                notificacaoBase = new EmailNotificacao();
                break;
            case TipoNotificacao.Sms:
                notificacaoBase = new SmsNotificacao();
                break;
            case TipoNotificacao.Push:
                notificacaoBase = new PushNotificacao();
                break;
            case TipoNotificacao.SmsExterno:
                notificacaoBase = new SmsExternoAdapter(new SmsExternoLegado());
                break;
            default:
                throw new ArgumentException("Tipo de notificação inválido.");
        }
        
        return new NotificacaoProxy(notificacaoBase);
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

        INotificacao notificacao4 = NotificacaoFactory.CriarNotificacao(TipoNotificacao.SmsExterno);
        notificacao4.Enviar("12999999999", "Mensagem enviada pelo serviço legado adaptado.");
    }
}