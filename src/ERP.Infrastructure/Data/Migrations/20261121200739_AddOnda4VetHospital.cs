using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ERP.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddOnda4VetHospital : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BaiasInternacao",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Codigo = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Setor = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ValorDiaria = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CriadoPor = table.Column<string>(type: "text", nullable: true),
                    AlteradoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AlteradoPor = table.Column<string>(type: "text", nullable: true),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaiasInternacao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BaiasInternacao_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CirurgiasVeterinarias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Numero = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    PetId = table.Column<int>(type: "integer", nullable: false),
                    ClienteId = table.Column<int>(type: "integer", nullable: false),
                    VeterinarioCirurgiaoId = table.Column<int>(type: "integer", nullable: false),
                    ComandaAtendimentoId = table.Column<int>(type: "integer", nullable: true),
                    AnestesistaNome = table.Column<string>(type: "text", nullable: true),
                    SalaCirurgica = table.Column<string>(type: "text", nullable: true),
                    Procedimento = table.Column<string>(type: "text", nullable: false),
                    DataHoraAgendada = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ProtocoloAnestesico = table.Column<string>(type: "text", nullable: true),
                    MonitoramentoTransOperatorio = table.Column<string>(type: "text", nullable: true),
                    Intercorrencias = table.Column<string>(type: "text", nullable: true),
                    ValorProcedimento = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CriadoPor = table.Column<string>(type: "text", nullable: true),
                    AlteradoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AlteradoPor = table.Column<string>(type: "text", nullable: true),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CirurgiasVeterinarias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CirurgiasVeterinarias_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CirurgiasVeterinarias_ComandasAtendimento_ComandaAtendiment~",
                        column: x => x.ComandaAtendimentoId,
                        principalTable: "ComandasAtendimento",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CirurgiasVeterinarias_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CirurgiasVeterinarias_Pets_PetId",
                        column: x => x.PetId,
                        principalTable: "Pets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CirurgiasVeterinarias_Veterinarios_VeterinarioCirurgiaoId",
                        column: x => x.VeterinarioCirurgiaoId,
                        principalTable: "Veterinarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExamesVeterinarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PetId = table.Column<int>(type: "integer", nullable: false),
                    VeterinarioSolicitanteId = table.Column<int>(type: "integer", nullable: true),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    NomeExame = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    CodigoAmostraLis = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    DataSolicitacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataResultado = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ResultadoTexto = table.Column<string>(type: "text", nullable: true),
                    LaudoUrl = table.Column<string>(type: "text", nullable: true),
                    PayloadHl7Bruto = table.Column<string>(type: "text", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CriadoPor = table.Column<string>(type: "text", nullable: true),
                    AlteradoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AlteradoPor = table.Column<string>(type: "text", nullable: true),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamesVeterinarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamesVeterinarios_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExamesVeterinarios_Pets_PetId",
                        column: x => x.PetId,
                        principalTable: "Pets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamesVeterinarios_Veterinarios_VeterinarioSolicitanteId",
                        column: x => x.VeterinarioSolicitanteId,
                        principalTable: "Veterinarios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TermosConsentimentoPet",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PetId = table.Column<int>(type: "integer", nullable: false),
                    ClienteId = table.Column<int>(type: "integer", nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    Titulo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ConteudoTermo = table.Column<string>(type: "text", nullable: false),
                    Assinado = table.Column<bool>(type: "boolean", nullable: false),
                    DataAssinatura = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AssinadoPorNome = table.Column<string>(type: "text", nullable: true),
                    TokenAssinatura = table.Column<string>(type: "text", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CriadoPor = table.Column<string>(type: "text", nullable: true),
                    AlteradoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AlteradoPor = table.Column<string>(type: "text", nullable: true),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TermosConsentimentoPet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TermosConsentimentoPet_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TermosConsentimentoPet_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TermosConsentimentoPet_Pets_PetId",
                        column: x => x.PetId,
                        principalTable: "Pets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InternacoesPet",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Numero = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    PetId = table.Column<int>(type: "integer", nullable: false),
                    ClienteId = table.Column<int>(type: "integer", nullable: false),
                    VeterinarioId = table.Column<int>(type: "integer", nullable: false),
                    BaiaInternacaoId = table.Column<int>(type: "integer", nullable: false),
                    ComandaAtendimentoId = table.Column<int>(type: "integer", nullable: true),
                    DataEntrada = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataAltaPrevista = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DataAltaReal = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MotivoInternacao = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ValorDiaria = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CriadoPor = table.Column<string>(type: "text", nullable: true),
                    AlteradoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AlteradoPor = table.Column<string>(type: "text", nullable: true),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InternacoesPet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InternacoesPet_BaiasInternacao_BaiaInternacaoId",
                        column: x => x.BaiaInternacaoId,
                        principalTable: "BaiasInternacao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InternacoesPet_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InternacoesPet_ComandasAtendimento_ComandaAtendimentoId",
                        column: x => x.ComandaAtendimentoId,
                        principalTable: "ComandasAtendimento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_InternacoesPet_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InternacoesPet_Pets_PetId",
                        column: x => x.PetId,
                        principalTable: "Pets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InternacoesPet_Veterinarios_VeterinarioId",
                        column: x => x.VeterinarioId,
                        principalTable: "Veterinarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EvolucoesInternacao",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InternacaoPetId = table.Column<int>(type: "integer", nullable: false),
                    DataHora = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Turno = table.Column<string>(type: "text", nullable: false),
                    ProfissionalNome = table.Column<string>(type: "text", nullable: false),
                    TemperaturaC = table.Column<decimal>(type: "numeric(4,1)", precision: 4, scale: 1, nullable: true),
                    FrequenciaCardiacaBpm = table.Column<int>(type: "integer", nullable: true),
                    FrequenciaRespiratoriaMpm = table.Column<int>(type: "integer", nullable: true),
                    EstadoGeral = table.Column<string>(type: "text", nullable: true),
                    DescricaoEvolucao = table.Column<string>(type: "text", nullable: false),
                    VisivelBoletimTutor = table.Column<bool>(type: "boolean", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CriadoPor = table.Column<string>(type: "text", nullable: true),
                    AlteradoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AlteradoPor = table.Column<string>(type: "text", nullable: true),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvolucoesInternacao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EvolucoesInternacao_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EvolucoesInternacao_InternacoesPet_InternacaoPetId",
                        column: x => x.InternacaoPetId,
                        principalTable: "InternacoesPet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PrescricoesInternacao",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InternacaoPetId = table.Column<int>(type: "integer", nullable: false),
                    ProdutoId = table.Column<int>(type: "integer", nullable: true),
                    DescricaoMedicamento = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Dose = table.Column<string>(type: "text", nullable: false),
                    Via = table.Column<string>(type: "text", nullable: true),
                    QuantidadePorAplicacao = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    IntervaloHoras = table.Column<int>(type: "integer", nullable: false),
                    ValorUnitarioCobranca = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    HorarioInicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CriadoPor = table.Column<string>(type: "text", nullable: true),
                    AlteradoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AlteradoPor = table.Column<string>(type: "text", nullable: true),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrescricoesInternacao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrescricoesInternacao_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PrescricoesInternacao_InternacoesPet_InternacaoPetId",
                        column: x => x.InternacaoPetId,
                        principalTable: "InternacoesPet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PrescricoesInternacao_Produtos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produtos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ChecagensEnfermagem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PrescricaoInternacaoId = table.Column<int>(type: "integer", nullable: false),
                    DataHoraPrevista = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataHoraAplicada = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ResponsavelEnfermagem = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_ChecagensEnfermagem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChecagensEnfermagem_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChecagensEnfermagem_PrescricoesInternacao_PrescricaoInterna~",
                        column: x => x.PrescricaoInternacaoId,
                        principalTable: "PrescricoesInternacao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BaiasInternacao_EmpresaId_Codigo",
                table: "BaiasInternacao",
                columns: new[] { "EmpresaId", "Codigo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChecagensEnfermagem_EmpresaId",
                table: "ChecagensEnfermagem",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_ChecagensEnfermagem_PrescricaoInternacaoId",
                table: "ChecagensEnfermagem",
                column: "PrescricaoInternacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_CirurgiasVeterinarias_ClienteId",
                table: "CirurgiasVeterinarias",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_CirurgiasVeterinarias_ComandaAtendimentoId",
                table: "CirurgiasVeterinarias",
                column: "ComandaAtendimentoId");

            migrationBuilder.CreateIndex(
                name: "IX_CirurgiasVeterinarias_EmpresaId_Numero",
                table: "CirurgiasVeterinarias",
                columns: new[] { "EmpresaId", "Numero" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CirurgiasVeterinarias_PetId",
                table: "CirurgiasVeterinarias",
                column: "PetId");

            migrationBuilder.CreateIndex(
                name: "IX_CirurgiasVeterinarias_VeterinarioCirurgiaoId",
                table: "CirurgiasVeterinarias",
                column: "VeterinarioCirurgiaoId");

            migrationBuilder.CreateIndex(
                name: "IX_EvolucoesInternacao_EmpresaId",
                table: "EvolucoesInternacao",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_EvolucoesInternacao_InternacaoPetId",
                table: "EvolucoesInternacao",
                column: "InternacaoPetId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamesVeterinarios_EmpresaId_CodigoAmostraLis",
                table: "ExamesVeterinarios",
                columns: new[] { "EmpresaId", "CodigoAmostraLis" });

            migrationBuilder.CreateIndex(
                name: "IX_ExamesVeterinarios_PetId",
                table: "ExamesVeterinarios",
                column: "PetId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamesVeterinarios_VeterinarioSolicitanteId",
                table: "ExamesVeterinarios",
                column: "VeterinarioSolicitanteId");

            migrationBuilder.CreateIndex(
                name: "IX_InternacoesPet_BaiaInternacaoId",
                table: "InternacoesPet",
                column: "BaiaInternacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_InternacoesPet_ClienteId",
                table: "InternacoesPet",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_InternacoesPet_ComandaAtendimentoId",
                table: "InternacoesPet",
                column: "ComandaAtendimentoId");

            migrationBuilder.CreateIndex(
                name: "IX_InternacoesPet_EmpresaId_Numero",
                table: "InternacoesPet",
                columns: new[] { "EmpresaId", "Numero" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InternacoesPet_PetId",
                table: "InternacoesPet",
                column: "PetId");

            migrationBuilder.CreateIndex(
                name: "IX_InternacoesPet_VeterinarioId",
                table: "InternacoesPet",
                column: "VeterinarioId");

            migrationBuilder.CreateIndex(
                name: "IX_PrescricoesInternacao_EmpresaId",
                table: "PrescricoesInternacao",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_PrescricoesInternacao_InternacaoPetId",
                table: "PrescricoesInternacao",
                column: "InternacaoPetId");

            migrationBuilder.CreateIndex(
                name: "IX_PrescricoesInternacao_ProdutoId",
                table: "PrescricoesInternacao",
                column: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_TermosConsentimentoPet_ClienteId",
                table: "TermosConsentimentoPet",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_TermosConsentimentoPet_EmpresaId",
                table: "TermosConsentimentoPet",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_TermosConsentimentoPet_PetId",
                table: "TermosConsentimentoPet",
                column: "PetId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChecagensEnfermagem");

            migrationBuilder.DropTable(
                name: "CirurgiasVeterinarias");

            migrationBuilder.DropTable(
                name: "EvolucoesInternacao");

            migrationBuilder.DropTable(
                name: "ExamesVeterinarios");

            migrationBuilder.DropTable(
                name: "TermosConsentimentoPet");

            migrationBuilder.DropTable(
                name: "PrescricoesInternacao");

            migrationBuilder.DropTable(
                name: "InternacoesPet");

            migrationBuilder.DropTable(
                name: "BaiasInternacao");
        }
    }
}
