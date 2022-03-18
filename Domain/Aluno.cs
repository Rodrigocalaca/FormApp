using System;
using System.Collections.Generic;

namespace ProjetoDeEstagio2
{
    public class Aluno : IEntidade
    {
        public int Matricula { get; set; }

        public string Nome { get; set; }

        public string CPF { get; set; }

        public DateTime Nascimento { get; set; }

        public EnumeradorSexo Sexo { get; set; }

        public Aluno(int matricula, string nome, string cpf, DateTime nascimento, EnumeradorSexo sexo)
        {
            Matricula = matricula;
            Nome = nome;
            CPF = cpf;
            Nascimento = nascimento;
            Sexo = sexo;
        }

        public Aluno() { }

        public override bool Equals(object obj)
        {
            return obj is Aluno aluno &&
                Matricula == aluno.Matricula &&
                Nome == aluno.Nome &&
                CPF == aluno.CPF &&
                Nascimento == aluno.Nascimento &&
                Sexo == aluno.Sexo;
        }

        public override int GetHashCode()
        {
            int hashCode = 2086685141;
            hashCode = hashCode * -1521134295 + Matricula.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Nome);
            hashCode = hashCode * -1521134295 + Sexo.GetHashCode();
            hashCode = hashCode * -1521134295 + Nascimento.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(CPF);
            return hashCode;
        }

        public override string ToString()
        {
            return $"Matrícula: {Matricula} - Nome: {Nome}";
        }
    }
}
