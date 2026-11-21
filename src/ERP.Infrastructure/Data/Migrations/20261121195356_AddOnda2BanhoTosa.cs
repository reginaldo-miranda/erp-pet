using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ERP.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddOnda2BanhoTosa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PacotesBanhoPet",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClienteId = table.Column<int>(type: "integer", nullable: false),
                    PetId = table.Column<int>(type: "integer", nullable: true),
                    NomePacote = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    TotalSessoes = table.Column<int>(type: "integer", nullable: false),
                    SessoesUtilizadas = table.Column<int>(type: "integer", nullable: false),
                    ValorTotal = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    DataInicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataValidade = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Observacoes = table.Column<string>(type: "text", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CriadoPor = table.Column<string>(type: "text", nullable: true),
                    AlteradoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AlteradoPor = table.Column<string>(type: "text", nullable: true),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PacotesBanhoPet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PacotesBanhoPet_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PacotesBanhoPet_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PacotesBanhoPet_Pets_PetId",
                        column: x => x.PetId,
                        principalTable: "Pets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "RecursosBanhoTosa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CriadoPor = table.Column<string>(type: "text", nullable: true),
                    AlteradoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AlteradoPor = table.Column<string>(type: "text", nullable: true),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecursosBanhoTosa", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecursosBanhoTosa_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TabelasPrecoBanho",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ServicoId = table.Column<int>(type: "integer", nullable: true),
                    NomeServico = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Porte = table.Column<int>(type: "integer", nullable: false),
                    TipoPelagem = table.Column<int>(type: "integer", nullable: false),
                    Valor = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    DuracaoMinutos = table.Column<int>(type: "integer", nullable: false),
                    ComissaoBanhoPercentual = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    ComissaoTosaPercentual = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CriadoPor = table.Column<string>(type: "text", nullable: true),
                    AlteradoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AlteradoPor = table.Column<string>(type: "text", nullable: true),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TabelasPrecoBanho", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TabelasPrecoBanho_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TabelasPrecoBanho_Servicos_ServicoId",
                        column: x => x.ServicoId,
                        principalTable: "Servicos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AtendimentosBanhoTosa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Numero = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    PetId = table.Column<int>(type: "integer", nullable: false),
                    ClienteId = table.Column<int>(type: "integer", nullable: false),
                    TabelaPrecoBanhoId = table.Column<int>(type: "integer", nullable: true),
                    PacoteBanhoPetId = table.Column<int>(type: "integer", nullable: true),
                    ComandaAtendimentoId = table.Column<int>(type: "integer", nullable: true),
                    RecursoBanhoTosaId = table.Column<int>(type: "integer", nullable: true),
                    BanhistaNome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    TosadorNome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DataAgendada = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DuracaoMinutos = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ValorServico = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    ValorAdicionais = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    ValorTotal = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CobertoPorPacote = table.Column<bool>(type: "boolean", nullable: false),
                    CheckInEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TemNos = table.Column<bool>(type: "boolean", nullable: false),
                    TemPulgasCarrapatos = table.Column<bool>(type: "boolean", nullable: false),
                    TemLesoesPele = table.Column<bool>(type: "boolean", nullable: false),
                    TemOtite = table.Column<bool>(type: "boolean", nullable: false),
                    Pertences = table.Column<string>(type: "text", nullable: true),
                    ObservacoesCheckIn = table.Column<string>(type: "text", nullable: true),
                    FotoEntradaUrl = table.Column<string>(type: "text", nullable: true),
                    FotoSaidaUrl = table.Column<string>(type: "text", nullable: true),
                    ProntoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EntregueEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CriadoPor = table.Column<string>(type: "text", nullable: true),
                    AlteradoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AlteradoPor = table.Column<string>(type: "text", nullable: true),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AtendimentosBanhoTosa", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AtendimentosBanhoTosa_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AtendimentosBanhoTosa_ComandasAtendimento_ComandaAtendiment~",
                        column: x => x.ComandaAtendimentoId,
                        principalTable: "ComandasAtendimento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_AtendimentosBanhoTosa_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AtendimentosBanhoTosa_PacotesBanhoPet_PacoteBanhoPetId",
                        column: x => x.PacoteBanhoPetId,
                        principalTable: "PacotesBanhoPet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_AtendimentosBanhoTosa_Pets_PetId",
                        column: x => x.PetId,
                        principalTable: "Pets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AtendimentosBanhoTosa_RecursosBanhoTosa_RecursoBanhoTosaId",
                        column: x => x.RecursoBanhoTosaId,
                        principalTable: "RecursosBanhoTosa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_AtendimentosBanhoTosa_TabelasPrecoBanho_TabelaPrecoBanhoId",
                        column: x => x.TabelaPrecoBanhoId,
                        principalTable: "TabelasPrecoBanho",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ComissoesBanhoTosa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AtendimentoBanhoTosaId = table.Column<int>(type: "integer", nullable: false),
                    ProfissionalNome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Etapa = table.Column<int>(type: "integer", nullable: false),
                    BaseCalculo = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Percentual = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    ValorComissao = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Paga = table.Column<bool>(type: "boolean", nullable: false),
                    DataPagamento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CriadoPor = table.Column<string>(type: "text", nullable: true),
                    AlteradoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AlteradoPor = table.Column<string>(type: "text", nullable: true),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComissoesBanhoTosa", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComissoesBanhoTosa_AtendimentosBanhoTosa_AtendimentoBanhoTo~",
                        column: x => x.AtendimentoBanhoTosaId,
                        principalTable: "AtendimentosBanhoTosa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComissoesBanhoTosa_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AtendimentosBanhoTosa_ClienteId",
                table: "AtendimentosBanhoTosa",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_AtendimentosBanhoTosa_ComandaAtendimentoId",
                table: "AtendimentosBanhoTosa",
                column: "ComandaAtendimentoId");

            migrationBuilder.CreateIndex(
                name: "IX_AtendimentosBanhoTosa_EmpresaId_Numero",
                table: "AtendimentosBanhoTosa",
                columns: new[] { "EmpresaId", "Numero" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AtendimentosBanhoTosa_PacoteBanhoPetId",
                table: "AtendimentosBanhoTosa",
                column: "PacoteBanhoPetId");

            migrationBuilder.CreateIndex(
                name: "IX_AtendimentosBanhoTosa_PetId",
                table: "AtendimentosBanhoTosa",
                column: "PetId");

            migrationBuilder.CreateIndex(
                name: "IX_AtendimentosBanhoTosa_RecursoBanhoTosaId",
                table: "AtendimentosBanhoTosa",
                column: "RecursoBanhoTosaId");

            migrationBuilder.CreateIndex(
                name: "IX_AtendimentosBanhoTosa_TabelaPrecoBanhoId",
                table: "AtendimentosBanhoTosa",
                column: "TabelaPrecoBanhoId");

            migrationBuilder.CreateIndex(
                name: "IX_ComissoesBanhoTosa_AtendimentoBanhoTosaId",
                table: "ComissoesBanhoTosa",
                column: "AtendimentoBanhoTosaId");

            migrationBuilder.CreateIndex(
                name: "IX_ComissoesBanhoTosa_EmpresaId",
                table: "ComissoesBanhoTosa",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_PacotesBanhoPet_ClienteId",
                table: "PacotesBanhoPet",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_PacotesBanhoPet_EmpresaId",
                table: "PacotesBanhoPet",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_PacotesBanhoPet_PetId",
                table: "PacotesBanhoPet",
                column: "PetId");

            migrationBuilder.CreateIndex(
                name: "IX_RecursosBanhoTosa_EmpresaId",
                table: "RecursosBanhoTosa",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_TabelasPrecoBanho_EmpresaId_Porte_TipoPelagem",
                table: "TabelasPrecoBanho",
                columns: new[] { "EmpresaId", "Porte", "TipoPelagem" });

            migrationBuilder.CreateIndex(
                name: "IX_TabelasPrecoBanho_ServicoId",
                table: "TabelasPrecoBanho",
                column: "ServicoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComissoesBanhoTosa");

            migrationBuilder.DropTable(
                name: "AtendimentosBanhoTosa");

            migrationBuilder.DropTable(
                name: "PacotesBanhoPet");

            migrationBuilder.DropTable(
                name: "RecursosBanhoTosa");

            migrationBuilder.DropTable(
                name: "TabelasPrecoBanho");
        }
    }
}
