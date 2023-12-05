using System;
using System.Collections.Generic;

class Program
{
    static Estoque estoque = new Estoque();
    public static Estoque GetEstoque()
    {
        return estoque;
    }
    static void Main()
    {
        int opcao;

        do
        {
            Console.WriteLine("Menu de Seleção:");
            Console.WriteLine("1. Cadastrar tipo de equipamento");
            Console.WriteLine("2. Consultar tipo de equipamento");
            Console.WriteLine("3. Cadastrar equipamento");
            Console.WriteLine("4. Registrar Contrato de Locação");
            Console.WriteLine("5. Consultar Contratos de Locação");
            Console.WriteLine("6. Liberar Contrato de Locação");
            Console.WriteLine("7. Consultar Contratos de Locação liberados");
            Console.WriteLine("8. Devolver equipamentos de Contrato de Locação liberado");
            Console.WriteLine("0. Sair");

            Console.Write("Escolha uma opção: ");
            opcao = int.Parse(Console.ReadLine());

            switch (opcao)
            {
                case 1:
                    Console.Write("Nome do equipamento: ");
                    string nome = Console.ReadLine();
                    Console.Write("ID do tipo de equipamento: ");
                    int idTipo = int.Parse(Console.ReadLine());
                    Console.Write("Valor de locação: ");
                    double valor = double.Parse(Console.ReadLine());

                    estoque.incluiProduto(nome, idTipo, valor);
                    break;

                case 2:
                    estoque.mostraProduto();
                    break;

                case 3:
                    Console.Write("ID do tipo de equipamento: ");
                    int idTipoCadastro = int.Parse(Console.ReadLine());
                    estoque.incluiItem(idTipoCadastro);
                    break;

                case 4:
                    estoque.adicionaContrato();
                    break;

                case 5:
                    estoque.mostrarContratos();
                    break;

                case 6:
                    Console.Write("ID do contrato: ");
                    int idContratoLiberar = int.Parse(Console.ReadLine());
                    estoque.liberarContrato(idContratoLiberar);
                    break;

                case 7:
                    estoque.mostrarContratosLiberados();
                    break;

                case 8:
                    Console.Write("ID do contrato: ");
                    int idContratoDevolver = int.Parse(Console.ReadLine());
                    estoque.devolver(idContratoDevolver);
                    break;

                default:
                    Console.WriteLine("Opção inválida. Tente novamente.");
                    break;
            }

        } while (opcao != 0);
    }
}

class Tipo
{
    public string nome;
    public int idTipo;
    public double valor;
    public Queue<Item> itens = new Queue<Item>();

    public void incluirItem()
    {
        int patrimonio = (itens.Count > 0) ? itens.Peek().patrimonio + 1 : 1;
        bool avariado = false;
        Item novoItem = new Item(patrimonio, avariado);
        itens.Enqueue(novoItem);
        Console.WriteLine("Item adicionado");
    }

    public void excluirItem()
    {
        if (itens.Count > 0)
        {
            itens.Dequeue();
        }
    }
}

class Item
{
    public int patrimonio;
    public bool avariado;

    public Item(int patrimonio, bool avariado)
    {
        this.patrimonio = patrimonio;
        this.avariado = avariado;
    }
}

class Estoque
{
    public List<Tipo> estoque = new List<Tipo>();
    public List<Contrato> contratos = new List<Contrato>();
    public List<Contrato> contratosLiberados = new List<Contrato>();

    public void incluiProduto(string nome, int idTipo, double valor)
    {
        Tipo tipoExistente = estoque.Find(t => t.idTipo == idTipo);

        if (tipoExistente == null)
        {
            Tipo novoTipo = new Tipo { nome = nome, idTipo = idTipo, valor = valor };
            estoque.Add(novoTipo);
            Console.WriteLine("Tipo de equipamento adicionado");
        }
        else
        {
            Console.WriteLine("Já há produto com esse Id");
        }
    }

    public void mostraProduto()
    {
        foreach (Tipo tipo in estoque)
        {
            Console.WriteLine($"Id: {tipo.idTipo}\nNome: {tipo.nome}\nValor Diária de Locação: {tipo.valor}");
        }
    }

    public void incluiItem(int idTipo)
    {
        Tipo tipoEncontrado = estoque.Find(t => t.idTipo == idTipo);

        if (tipoEncontrado != null)
        {
            tipoEncontrado.incluirItem();
        }
        else
        {
            Console.WriteLine("Produto não encontrado");
        }
    }

    public void excluirItem(int idTipo)
    {
        Tipo tipoEncontrado = estoque.Find(t => t.idTipo == idTipo);

        if (tipoEncontrado != null)
        {
            tipoEncontrado.excluirItem();
        }
        else
        {
            Console.WriteLine("Produto não encontrado");
        }
    }

    public void adicionaContrato()
    {
        int idContrato = (contratos.Count > 0) ? contratos[contratos.Count - 1].idContrato + 1 : 1;

        Console.Write("Data de retirada: ");
        DateTime saida = DateTime.Parse(Console.ReadLine());
        Console.Write("Data de devolução: ");
        DateTime retorno = DateTime.Parse(Console.ReadLine());

        Contrato novoContrato = new Contrato(idContrato, saida, retorno);
        contratos.Add(novoContrato);

        do
        {
            Console.Write("ID do tipo de equipamento a ser adicionado (0 para finalizar): ");
            int idTipo = int.Parse(Console.ReadLine());

            if (idTipo == 0)
            {
                break;
            }

            Tipo tipoEncontrado = estoque.Find(t => t.idTipo == idTipo);

            if (tipoEncontrado != null)
            {
                int quantidade;
                do
                {
                    Console.Write("Quantidade a ser adicionada: ");
                    quantidade = int.Parse(Console.ReadLine());

                    if (quantidade <= 0)
                    {
                        Console.WriteLine("Quantidade inválida. Tente novamente.");
                    }

                } while (quantidade <= 0);


                for (int i = 0; i < quantidade; i++)
                {
                    while (tipoEncontrado.itens.Count > 0)
                    {
                        Item item = tipoEncontrado.itens.Dequeue(); // Remove o primeiro item da fila

                        if (!item.avariado)
                        {
                            novoContrato.produtos.Push(tipoEncontrado); // Adiciona o tipo à pilha
                            break;
                        }
                        else
                        {
                            tipoEncontrado.itens.Enqueue(item); // Adiciona o item avariado de volta ao final da fila
                        }
                    }

                    if (tipoEncontrado.itens.Count == 0)
                    {
                        Console.WriteLine("Todos os itens estão avariados. Não é possível adicionar ao contrato.");
                        break;
                    }
                }
            }
            else
            {
                Console.WriteLine("Produto não encontrado");
            }

        } while (true);
    }


    public void mostrarContratos()
    {
        foreach (Contrato contrato in contratos)
        {
            Console.WriteLine($"Contrato: {contrato.idContrato}\nProdutos:");

            foreach (Tipo tipo in contrato.produtos)
            {
                Console.WriteLine($"Id: {tipo.idTipo}\nNome: {tipo.nome}\nValor Diária de Locação: {tipo.valor}\nQuantidade: {tipo.itens.Count}");
            }
        }
    }

    public double valorContrato()
    {
        if (contratosLiberados.Count > 0)
        {
            Contrato ultimoContrato = contratosLiberados[contratosLiberados.Count - 1];
            int dias = (int)(ultimoContrato.retorno - ultimoContrato.saida).TotalDays;
            double valorTotal = 0;

            foreach (Tipo tipo in ultimoContrato.produtos)
            {
                valorTotal += dias * tipo.valor * tipo.itens.Count;
            }

            return valorTotal;
        }
        else
        {
            return 0; 
        }
    }

    public void liberarContrato(int IdContrato)
    {
        Contrato contratoEncontrado = contratos.Find(c => c.idContrato == IdContrato);

        if (contratoEncontrado != null)
        {
            contratosLiberados.Add(contratoEncontrado);
            contratos.Remove(contratoEncontrado);
            Console.WriteLine("Contrato liberado");
        }
        else
        {
            Console.WriteLine("Contrato não encontrado");
        }
    }

    public void devolver(int idContrato)
    {
        Contrato contratoEncontrado = contratosLiberados.Find(c => c.idContrato == idContrato);

        if (contratoEncontrado != null)
        {
            Console.WriteLine($"Valor do contrato: {valorContrato()}");
            Console.WriteLine("Devolvendo equipamentos...");

            foreach (Tipo tipo in contratoEncontrado.produtos)
            {
                List<Item> itensDevolvidos = new List<Item>();

                while (tipo.itens.Count > 0)
                {
                    Item item = tipo.itens.Dequeue();

                    Console.Write($"O item {item.patrimonio} está avariado? (S/N): ");
                    String resposta = Console.ReadLine();

                    if (resposta.Equals("S", StringComparison.OrdinalIgnoreCase))
                    {
                        item.avariado = true;
                    }

                    itensDevolvidos.Add(item);
                }

                foreach (Item itemDevolvido in itensDevolvidos)
                {
                    estoque.Find(t => t.idTipo == tipo.idTipo).itens.Enqueue(itemDevolvido);
                }
            }

            contratosLiberados.Remove(contratoEncontrado);
            Console.WriteLine("Equipamentos devolvidos");
        }
        else
        {
            Console.WriteLine("Contrato não encontrado");
        }
    }



    public void mostrarContratosLiberados()
    {
        foreach (Contrato contrato in contratosLiberados)
        {
            Console.WriteLine($"Contrato: {contrato.idContrato}\nProdutos:");

            foreach (Tipo tipo in contrato.produtos)
            {
                Console.WriteLine($"Id: {tipo.idTipo}\nNome: {tipo.nome}\nValor Diária de Locação: {tipo.valor}\nQuantidade: {tipo.itens.Count}");
            }
        }
    }
}

class Contrato
{
    public int idContrato;
    public DateTime saida;
    public DateTime retorno;
    public Stack<Tipo> produtos = new Stack<Tipo>();

    public Contrato(int idContrato, DateTime saida, DateTime retorno)
    {
        this.idContrato = idContrato;
        this.saida = saida;
        this.retorno = retorno;
    }

    public void adicionaProdutos(int idTipo, int qtd)
    {
        Tipo tipoEncontrado = Program.GetEstoque().estoque.Find(t => t.idTipo == idTipo);

        if (tipoEncontrado != null)
        {
            Tipo copiaTipo = new Tipo { nome = tipoEncontrado.nome, idTipo = tipoEncontrado.idTipo, valor = tipoEncontrado.valor };

            for (int i = 0; i < qtd; i++)
            {
                if (copiaTipo.itens.Count > 0 && !copiaTipo.itens.Peek().avariado)
                {
                    Program.GetEstoque().excluirItem(idTipo);
                    produtos.Push(tipoEncontrado);
                }
                else if (copiaTipo.itens.Count > 0)
                {
                    Program.GetEstoque().excluirItem(idTipo);
                    produtos.Push(tipoEncontrado);
                }
            }
        }
        else
        {
            Console.WriteLine("Produto não encontrado");
        }
    }
}
