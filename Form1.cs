using EM.Repository;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace ProjetoDeEstagio2
{
    public partial class Form1 : Form
    {
        readonly RepositorioAluno repositorio = new RepositorioAluno();
        public Form1()
        {
            InitializeComponent();
            PreenchaGrid();
            BtnPesquisar.Enabled = false;
        }

        public void PreenchaGrid()
        {
            //dataGridView1.DataSource = new BindingSource();
            dataGridView1.DataSource = repositorio.GetAll().ToSortableBindingList();
            AjustaTamanhoDasColunasPersonalizado();
        }

        private Aluno PreenchaAluno()
        {
            string nome = txtNome.Text.Trim().ToLower();
            TextInfo info = Thread.CurrentThread.CurrentCulture.TextInfo;
            nome = info.ToTitleCase(nome);

            string cpf = txtCPF.Text.Replace(".", "").Replace("-", "");

            Aluno aluno = new Aluno(
            Convert.ToInt32(txtMatricula.Text),
            nome,
            cpf,
            Convert.ToDateTime(escolhaNascimento.Text),
            (EnumeradorSexo)Enum.Parse(typeof(EnumeradorSexo), escolhaSexo.Text));
            return aluno;
        }

        private void MudaEstadoBotaoAdicionar()
        {
            BtnAdicionar.Text = "Adicionar";
            BtnLimpar.Text = "Limpar";
            groupBox1.Text = "Novo Aluno";
            txtMatricula.ReadOnly = false;
            LimpaCampos();
        }

        private void LimpaCampos()
        {
            txtMatricula.Clear();
            txtNome.Clear();
            txtBusca.Clear();
            escolhaNascimento.Clear();
            txtCPF.Clear();
            escolhaSexo.SelectedIndex = -1;
        }

        private bool EhCPFValido(string cpf)
        {
            string valor = txtCPF.Text.Replace(".", "").Replace("-", "");

            if (valor.Length != 11)
            {
                return false;
            }

            bool igual = true;

            for (int i = 1; i < 11 && igual; i++)
            {
                if (valor[i] != valor[0])
                {
                    igual = false;
                }
            }
            if (igual || valor == "12345678909")
            {
                return false;
            }

            int[] numeros = new int[11];

            for (int i = 0; i < 11; i++)
            {
                numeros[i] = int.Parse(valor[i].ToString());
            }

            int soma = 0;

            for (int i = 0; i < 9; i++)
            {
                soma += (10 - i) * numeros[i];
            }

            int resultado = soma % 11;



            if (resultado == 1 || resultado == 0)
            {
                if (numeros[9] != 0)
                {
                    return false;
                }
            }

            else if (numeros[9] != 11 - resultado)
            {
                return false;
            }

            soma = 0;

            for (int i = 0; i < 10; i++)
            {
                soma += (11 - i) * numeros[i];
            }

            resultado = soma % 11;


            if (resultado == 1 || resultado == 0)
            {
                if (numeros[10] != 0)
                {
                    return false;
                }

            }

            else
            {
                if (numeros[10] != 11 - resultado)
                {
                    return false;
                }
            }
            return true;
        }

        public bool EhValido()
        {
            if (TemCampoVazio())
            {
                return false;
            }
            DateTime dataNascimentoMinima = new DateTime(1900, 01, 01, 00, 00, 00);
            DateTime dataDeTesteAluno = Convert.ToDateTime(escolhaNascimento.Text);
            DateTime dataAtual = DateTime.Now;
            int CompararDatas = DateTime.Compare(dataNascimentoMinima, dataDeTesteAluno);
            int idade = dataAtual.Year - dataDeTesteAluno.Year;
            if (CompararDatas > 0 || dataAtual.Year < dataDeTesteAluno.Year )
            {
                MessageBox.Show("Insira uma data válida", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                escolhaNascimento.Clear();
                escolhaNascimento.Focus();
                return false;
            }
            int mesNascimento = Convert.ToInt32(dataDeTesteAluno.Month) + 12;
            int mesAtual = Convert.ToInt32(dataAtual.Month) + 12;
            if (Math.Abs(mesAtual - mesNascimento) >= 7 && idade <= 1)
            {
                MessageBox.Show("Idade insuficiente para o cadastro", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                escolhaNascimento.Clear();
                escolhaNascimento.Focus();
                return false;
            }
            int numeroDaMatricula = Convert.ToInt32(txtMatricula.Text);
            if ((JaTemEssaMatricula() && BtnAdicionar.Text == "Adicionar"))
            {
                MessageBox.Show("Matrícula ja cadastrada", "ERRO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMatricula.Clear();
                txtMatricula.Focus();
                return false;
            }

            if (numeroDaMatricula <= 0)
            {
                MessageBox.Show("Matrícula não pode ser 0", "ERRO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMatricula.Clear();
                txtMatricula.Focus();
                return false;
            }
            
            if (!Regex.IsMatch(txtNome.Text, @"^[\p{L}\p{M}' \.\-]+$"))
            {
                MessageBox.Show("Insira um nome valido", "ERRO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNome.Clear();
                txtNome.Focus();
                return false;
            }
            
            if (txtNome.TextLength < 1)
            {
                MessageBox.Show("O campo NOME precisa conter pelo menos um caracter", "ERRO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNome.Clear();
                txtNome.Focus();
                return false;
            }
            
            else if (!escolhaNascimento.MaskCompleted)
            {
                MessageBox.Show("A DATA DE NASCIMENTO precisa ser valida", "ERRO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                escolhaNascimento.Clear();
                escolhaNascimento.Focus();
                return false;
            }
            if (JaTemEsseCPF())
            {
                MessageBox.Show("CPF já cadastrado", "ERRO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCPF.Clear();
                txtCPF.Focus();
                return false;
            }
            if(txtCPF.Text.Length != 0)
            {
                if (Regex.IsMatch(txtCPF.Text, @"^[\p{L}\p{M}' \.\-]+$") || !EhCPFValido(txtCPF.Text))
                {
                    MessageBox.Show("O CPF é invalido", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCPF.Focus();
                    txtCPF.Clear();
                    return false;
                }
            }

            return true;
        }

        private bool JaTemEsseCPF()
        {
            string cpf = txtCPF.Text.Replace(".", "").Replace("-", "");
            if (cpf == String.Empty)
            {
                return false;
            }

            IEnumerable<Aluno> alunos = repositorio.Get(alunosCPF => alunosCPF.CPF == cpf && alunosCPF.Matricula != Convert.ToInt32(txtMatricula.Text));
            return alunos.Any();
        }

        private bool JaTemEssaMatricula()
        {
            if (txtMatricula.Text == String.Empty)
            {
                return false;
            }

            IEnumerable<Aluno> alunos = repositorio.Get(alunosMatricula => alunosMatricula.Matricula == Convert.ToInt32(txtMatricula.Text));
            return alunos.Any();
        }

        private void BtnAdicionar_Click_1(object sender, EventArgs e)
        {
            
            try
            {
                if (!EhValido())
                {
                    return;
                }
                Aluno aluno = PreenchaAluno();

                if (BtnAdicionar.Text == "Adicionar")
                {
                    repositorio.Add(aluno);
                    MessageBox.Show("Aluno adicionado", "SUCESSO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    repositorio.Update(aluno);
                }
                if (BtnAdicionar.Text == "Modificar")
                {
                    MudaEstadoBotaoAdicionar();
                }
                PreenchaGrid();
                LimpaCampos();
            }
            catch (Exception)
            {
                MessageBox.Show("Houve um erro ao inserir os dados. Por favor insira novamente", "ERRO", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnLimpar_Click_1(object sender, EventArgs e)
        {
            LimpaCampos();

            if (BtnLimpar.Text == "Cancelar")
            {
                MudaEstadoBotaoAdicionar();
            }
        }

        private void BtnEditar_Click_1(object sender, EventArgs e)
        {
            if (dataGridView1.RowCount >= 1)
            {
                BtnAdicionar.Text = "Modificar";
                BtnLimpar.Text = "Cancelar";
                groupBox1.Text = "Editando aluno";
                txtMatricula.ReadOnly = true;

                txtMatricula.Text = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                txtNome.Text = dataGridView1.CurrentRow.Cells[1].Value.ToString();
                txtCPF.Text = dataGridView1.CurrentRow.Cells[2].Value.ToString();
                escolhaNascimento.Text = dataGridView1.CurrentRow.Cells[3].Value.ToString();
                escolhaSexo.SelectedItem = dataGridView1.CurrentRow.Cells[4].Value.ToString();
            }
        }

        private void BtnExcluir_Click_1(object sender, EventArgs e)
        {
            Aluno aluno = new Aluno();

            if (dataGridView1.Rows.Count != 0)
            {
                DialogResult dr = MessageBox.Show("Deseja excluir o aluno?", "AVISO", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dr == DialogResult.Yes)
                {
                    aluno.Matricula = Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value.ToString());
                    repositorio.Remove(aluno);
                    PreenchaGrid();
                }
            }
            MudaEstadoBotaoAdicionar();
        }

        private void BtnPesquisar_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtBusca.Text, out int Pesquisa))
            {
                List<Aluno> alunos = new List<Aluno>
                {
                    repositorio.GetByMatricula(Pesquisa)
                };
                TryExecptDaPesquisa(alunos);
                
            }
            else if (repositorio.GetByNome(txtBusca.Text).Any())
            {
                var alunos = repositorio.GetByNome(txtBusca.Text).ToList();
                TryExecptDaPesquisa(alunos);
            }
            else
            {
                MessageBox.Show("Aluno não encontrado", "ERRO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBusca.Clear();
            }
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == 2 && (string)e.Value != string.Empty)
            {
                double.TryParse(e.Value.ToString(), out double d);
                e.Value = d.ToString(@"000\.000\.000-00");
            }
        }

        private void txtBusca_TextChanged(object sender, EventArgs e)
        {
            if (txtBusca.Text.Length == 0)
            {
                PreenchaGrid();
                BtnPesquisar.Enabled = false;
            }
            else
            {
                BtnPesquisar.Enabled = true;
            }
        }

        private void escolhaNascimento_TypeValidationCompleted(object sender, TypeValidationEventArgs e)
        {
            if (!e.IsValidInput)
            {
                MessageBox.Show("Data inválida", "ERRO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                escolhaNascimento.Clear();
            }
        }

        private void txtMatricula_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                AjustaTamanhoDasColunasFill();
            }
            else
            {
                AjustaTamanhoDasColunasFill();
            }
        }

        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                AjustaTamanhoDasColunasFill();
            }
            else
            {
                AjustaTamanhoDasColunasPersonalizado();
            }
        }

        public void AjustaTamanhoDasColunasPersonalizado()
        {
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                if (column.DataPropertyName == "Matricula")
                {
                    column.Width = 90;
                }
                if (column.DataPropertyName == "Nome")
                {
                    column.Width = 345;
                }
                if (column.DataPropertyName == "CPF")
                {
                    column.Width = 100;
                }
                if (column.DataPropertyName == "Nascimento")
                {
                    column.Width = 90;
                }
                if (column.DataPropertyName == "Sexo")
                {
                    column.Width = 90;
                }
            }
        }

        public void AjustaTamanhoDasColunasFill()
        {
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                AjustaTamanhoDasColunasPersonalizado();
            }
        }

        public void TryExecptDaPesquisa(List<Aluno> alunos)
        {
            try
            {
                // dataGridView1.DataSource = new BindingSource();
                dataGridView1.DataSource = alunos.ToList().ToSortableBindingList();
            }
            catch (Exception)
            {
                MessageBox.Show("Insira novamente os dados de pesquisa", "ERRO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBusca.Clear();
            }
        }

        public bool TemCampoVazio()
        {
            bool temCampoVazio = false;
            List<string> pegaInfo = new List<string>();
            if (txtMatricula.Text.Equals(""))
            {
                pegaInfo.Add("MATRICULA");
                temCampoVazio = true;
            }
            if (string.IsNullOrEmpty(txtNome.Text.TrimStart()))
            {
                pegaInfo.Add("NOME");
                temCampoVazio = true;
            }
            if (escolhaSexo.SelectedIndex == -1)
            {
                pegaInfo.Add("SEXO");
                temCampoVazio = true;
            }
            string nascimento = escolhaNascimento.Text;
            if (nascimento == "  /  /")
            {
                pegaInfo.Add("NASCIMENTO");
                temCampoVazio = true;
            }
            if (temCampoVazio)
            {
                string mensagem = string.Join(", ", pegaInfo);
                MessageBox.Show($"Os seguintes campos obrigatórios estão vazios: {mensagem}", "AVISO", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return true;
            }
            return false;         
        }       
    }
}

///coisas a fazer:
///refatorar o código
///

///coisas a se estudar:
///binding source