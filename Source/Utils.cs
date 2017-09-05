/* 
 * GDA - Generics Data Access, is framework to object-relational mapping 
 * (a programming technique for converting data between incompatible 
 * type systems in databases and Object-oriented programming languages) using c#.
 * 
 * Copyright (C) 2010  <http://www.colosoft.com.br/gda> - support@colosoft.com.br
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Text;
using GDA.Sql.InterpreterExpression;
using GDA.Sql.InterpreterExpression.Grammar;
using GDA.Sql.InterpreterExpression.Nodes;

namespace GDA.Sql
{
	internal class Utils
	{
		public static void Test()
		{
			string c = "select a.*, f.nome as nomefuncionario from atendimento a " + "inner join funcionarios f on a.idn_funcionario = f.idn_funcionario " + "where a.sta_reg <> 'R' order by a.dta_cadastro";
			string c2 = "select mAX(p.Idn_Item), p.idn_item, (1 + 3 + 3) as teste, p.idn_pesquisa, f.nome as funcionario, o.nome as operadora," + "p.descricao, p.dta_cadastro, p.sta_reg, p.dta_pesquisa from pesquisa p " + "inner join funcionarios f on (p.idn_funcionario=f.idn_funcionario) " + "inner join Operadora o on (p.idn_operadora=o.idn_operadora) " + "where p.idn_pesquisa=1 group by p.idn_pesquisa, f.Nome having p.nom=2 order by p.dta_cadastro";
			string ss = "SELECT Func(Func2(Model.a.CodName),(()), (1+2)+3, 'dada') AS QQ, a.teste K FROM " + "(SELECT tb1 from cidades cid) as t";
			string[] cmds = new string[] {
				c2,
				c,
				ss,
				"SELECT * FROM    Produtos WHERE    PrecoUnidade    ANY    (    SELECT    PrecoUnidade    FROM    DetalhePedido    WHERE    Desconto = 0.25 ) ",
				"SELECT Sobrenome, Nome, Titulo, Salario FROM   Empregados AS T1 WHERE    Salario =    (    SELECT     Avg(Salario)    FROM    Empregados    WHERE    T1.Titulo = Empregados.Titulo    ) ORDER BY Titulo ",
				"SELECT Sobrenomes, Nome, Cargo, Salario FROM Empregados WHERE Cargo LIKE 'Agente Ven*' AND Salario ALL ( SELECT    Salario    FROM    Empregados     WHERE    Cargo LIKE '*Chefe*'    OR    Cargo LIKE '*Diretor*' ) ",
				"SELECT DISTINCT NomeProduto, Preco_Unidade FROM    Produtos WHERE     PrecoUnidade = (    SELECT    PrecoUnidade    FROM    Produtos     WHERE    NomeProduto = 'Almíbar anisado' ) ",
				"SELECT DISTINCT     NomeContato, NomeCompanhia, CargoContato, Telefone FROM    Clientes WHERE    IdCliente NOT IN (     SELECT DISTINCT IdCliente    FROM Pedidos    WHERE DataPedido <#07/01/1993# ) ",
				"SELECT    Nome, Sobrenomes FROM     Empregados AS E WHERE EXISTS     (     SELECT *     FROM    Pedidos AS O    WHERE O.IdEmpregado = E.IdEmpregado ) ",
				"SELECT DISTINCT    Pedidos.Id_Produto, Pedidos.Quantidade,     (    SELECT    Produtos.Nome     FROM    Produtos   WHERE     Produtos.IdProduto = Pedidos.IdProduto    ) AS ElProduto FROM    Pedidos WHERE    Pedidos.Quantidade = 150 ORDER BY   Pedidos.Id_Produto ",
				"SELECT     NumVoo, Lugares FROM    Voos WHERE    Origem = 'Madri'    AND Exists (    SELECT T1.NumVoo FROM Voos AS T1    WHERE T1.LuagaresLivres > 0 AND T1.NumVuelo=Vuelos.NumVuelo) ",
				"SELECT    PedidosPendentes.Nome FROM     PedidosPendentes GROUP BY     PedidosPendentes.Nome HAVING    SUM(PedidosPendentes.Quantidade <    (    SELECT    Produtos.Estoque    FROM    Produtos     WHERE     Produtos.IdProduto = PedidosPendentes.IdProduto    ) ) ",
				"SELECT     Empregados.Nome FROM     Empregados WHERE    Sexo = 'M' AND Idade > ANY     (SELECT Empregados.Idade FROM Empregados WHERE Sexo ='H') ",
				"SELECT    Empregados.Nome FROM     Empregados WHERE     Sexo = 'M' AND Idade >    (SELECT Max( Empregados.Idade )FROM Empregados WHERE Sexo ='H') ",
				"SELECT * FROM Persons WHERE LastName BETWEEN 'Hansen' AND 'Pettersen'",
				"SELECT @string:=Full_Name, SUBSTRING(@string, LENGTH(SUBSTRING(REVERSE(@string), LOCATE(' ', REVERSE(@string))-1))) AS Last_Name FROM contacts",
				"SELECT `ip`, SUBSTRING_INDEX(`ip`, '.', 1) AS a, SUBSTRING_INDEX(SUBSTRING_INDEX(`ip`, '.', 2), '.', -1) AS b, SUBSTRING_INDEX(SUBSTRING_INDEX(`ip`, '.', -2), '.', 1) AS c, SUBSTRING_INDEX(`ip`, '.', -1) AS d FROM log_table",
				@"select se1.rank,w2.lemma
from word w1
left join sense se1 on w1.wordid = se1.wordid
left join synset sy1 on se1.synsetid = sy1.synsetid
left join semlinkref on sy1.synsetid = semlinkref.synset1id
left join synset sy2 on semlinkref.synset2id = sy2.synsetid
left join sense se2 on sy2.synsetid = se2.synsetid
left join word w2 on se2.wordid = w2.wordid
where w1.lemma = 'horse'
and sy1.pos = 'n'
and semlinkref.linkid = 1
order by se1.rank asc;"
			};
			foreach (string cmd in cmds)
			{
				SelectStatement s1 = SqlBuilder.P(cmd);
				SelectStatement s2 = SqlBuilder.P(s1.Parser());
				string[] sql = new string[] {
					s1.Parser().Replace(" ", "").ToUpper(),
					s2.Parser().Replace(" ", "").ToUpper(),
					cmd.Replace(" ", "").ToUpper()
				};
				if(s1 != s2 || (sql[0] != sql[1] && sql[1] != sql[2]))
				{
					throw new Exception("Diferentes.");
				}
				else
					Console.WriteLine("Iguais");
				System.Diagnostics.Debug.WriteLine(sql[0] + "\n" + sql[1] + "\n" + sql[2]);
			}
		}
	}
}
