using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ERP.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddOnda3VetAmbulatorio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Veterinarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Crmv = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    UfCrmv = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    Especialidade = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Telefone = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    ValorConsultaPadrao = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CriadoPor = table.Column<string>(type: "text", nullable: true),
                    AlteradoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AlteradoPor = table.Column<string>(type: "text", nullable: true),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Veterinarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Veterinarios_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConsultasVeterinarias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Numero = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    PetId = table.Column<int>(type: "integer", nullable: false),
                    ClienteId = table.Column<int>(type: "integer", nullable: false),
                    VeterinarioId = table.Column<int>(type: "integer", nullable: false),
                    ComandaAtendimentoId = table.Column<int>(type: "integer", nullable: true),
                    DataHora = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    QueixaPrincipal = table.Column<string>(type: "text", nullable: true),
                    PesoKg = table.Column<decimal>(type: "numeric(8,3)", precision: 8, scale: 3, nullable: true),
                    TemperaturaC = table.Column<decimal>(type: "numeric(4,1)", precision: 4, scale: 1, nullable: true),
                    FrequenciaCardiacaBpm = table.Column<int>(type: "integer", nullable: true),
                    FrequenciaRespiratoriaMpm = table.Column<int>(type: "integer", nullable: true),
                    Mucosas = table.Column<string>(type: "text", nullable: true),
                    TpcSegundos = table.Column<int>(type: "integer", nullable: true),
                    SoapSubjetivo = table.Column<string>(type: "text", nullable: true),
                    SoapObjetivo = table.Column<string>(type: "text", nullable: true),
                    SoapAvaliacao = table.Column<string>(type: "text", nullable: true),
                    SoapPlano = table.Column<string>(type: "text", nullable: true),
                    ValorConsulta = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    FinalizadaEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CriadoPor = table.Column<string>(type: "text", nullable: true),
                    AlteradoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AlteradoPor = table.Column<string>(type: "text", nullable: true),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsultasVeterinarias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConsultasVeterinarias_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConsultasVeterinarias_ComandasAtendimento_ComandaAtendiment~",
                        column: x => x.ComandaAtendimentoId,
                        principalTable: "ComandasAtendimento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ConsultasVeterinarias_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConsultasVeterinarias_Pets_PetId",
                        column: x => x.PetId,
                        principalTable: "Pets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConsultasVeterinarias_Veterinarios_VeterinarioId",
                        column: x => x.VeterinarioId,
                        principalTable: "Veterinarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VacinacoesVermifugosPet",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PetId = table.Column<int>(type: "integer", nullable: false),
                    VeterinarioId = table.Column<int>(type: "integer", nullable: true),
                    ProdutoId = table.Column<int>(type: "integer", nullable: true),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    NomeProduto = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Fabricante = table.Column<string>(type: "text", nullable: true),
                    NumeroLote = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    ValidadeLote = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Dose = table.Column<string>(type: "text", nullable: true),
                    DataAplicacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProximaAplicacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
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
                    table.PrimaryKey("PK_VacinacoesVermifugosPet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VacinacoesVermifugosPet_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VacinacoesVermifugosPet_Pets_PetId",
                        column: x => x.PetId,
                        principalTable: "Pets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VacinacoesVermifugosPet_Produtos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produtos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_VacinacoesVermifugosPet_Veterinarios_VeterinarioId",
                        column: x => x.VeterinarioId,
                        principalTable: "Veterinarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ReceitasVeterinarias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ConsultaVeterinariaId = table.Column<int>(type: "integer", nullable: true),
                    PetId = table.Column<int>(type: "integer", nullable: false),
                    VeterinarioId = table.Column<int>(type: "integer", nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    DataEmissao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
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
                    table.PrimaryKey("PK_ReceitasVeterinarias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReceitasVeterinarias_ConsultasVeterinarias_ConsultaVeterina~",
                        column: x => x.ConsultaVeterinariaId,
                        principalTable: "ConsultasVeterinarias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ReceitasVeterinarias_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReceitasVeterinarias_Pets_PetId",
                        column: x => x.PetId,
                        principalTable: "Pets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReceitasVeterinarias_Veterinarios_VeterinarioId",
                        column: x => x.VeterinarioId,
                        principalTable: "Veterinarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReceitaVeterinariaItens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ReceitaVeterinariaId = table.Column<int>(type: "integer", nullable: false),
                    Medicamento = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    PrincipioAtivo = table.Column<string>(type: "text", nullable: true),
                    Concentracao = table.Column<string>(type: "text", nullable: true),
                    Posologia = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    ViaAdministracao = table.Column<string>(type: "text", nullable: true),
                    DuracaoDias = table.Column<int>(type: "integer", nullable: true),
                    Quantidade = table.Column<string>(type: "text", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CriadoPor = table.Column<string>(type: "text", nullable: true),
                    AlteradoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AlteradoPor = table.Column<string>(type: "text", nullable: true),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceitaVeterinariaItens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReceitaVeterinariaItens_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReceitaVeterinariaItens_ReceitasVeterinarias_ReceitaVeterin~",
                        column: x => x.ReceitaVeterinariaId,
                        principalTable: "ReceitasVeterinarias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConsultasVeterinarias_ClienteId",
                table: "ConsultasVeterinarias",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultasVeterinarias_ComandaAtendimentoId",
                table: "ConsultasVeterinarias",
                column: "ComandaAtendimentoId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultasVeterinarias_EmpresaId_Numero",
                table: "ConsultasVeterinarias",
                columns: new[] { "EmpresaId", "Numero" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConsultasVeterinarias_PetId",
                table: "ConsultasVeterinarias",
                column: "PetId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultasVeterinarias_VeterinarioId",
                table: "ConsultasVeterinarias",
                column: "VeterinarioId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceitasVeterinarias_ConsultaVeterinariaId",
                table: "ReceitasVeterinarias",
                column: "ConsultaVeterinariaId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceitasVeterinarias_EmpresaId",
                table: "ReceitasVeterinarias",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceitasVeterinarias_PetId",
                table: "ReceitasVeterinarias",
                column: "PetId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceitasVeterinarias_VeterinarioId",
                table: "ReceitasVeterinarias",
                column: "VeterinarioId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceitaVeterinariaItens_EmpresaId",
                table: "ReceitaVeterinariaItens",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceitaVeterinariaItens_ReceitaVeterinariaId",
                table: "ReceitaVeterinariaItens",
                column: "ReceitaVeterinariaId");

            migrationBuilder.CreateIndex(
                name: "IX_VacinacoesVermifugosPet_EmpresaId",
                table: "VacinacoesVermifugosPet",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_VacinacoesVermifugosPet_PetId",
                table: "VacinacoesVermifugosPet",
                column: "PetId");

            migrationBuilder.CreateIndex(
                name: "IX_VacinacoesVermifugosPet_ProdutoId",
                table: "VacinacoesVermifugosPet",
                column: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_VacinacoesVermifugosPet_VeterinarioId",
                table: "VacinacoesVermifugosPet",
                column: "VeterinarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Veterinarios_EmpresaId",
                table: "Veterinarios",
                column: "EmpresaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReceitaVeterinariaItens");

            migrationBuilder.DropTable(
                name: "VacinacoesVermifugosPet");

            migrationBuilder.DropTable(
                name: "ReceitasVeterinarias");

            migrationBuilder.DropTable(
                name: "ConsultasVeterinarias");

            migrationBuilder.DropTable(
                name: "Veterinarios");
        }
    }
}
