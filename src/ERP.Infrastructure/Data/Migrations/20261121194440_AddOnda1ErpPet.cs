using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ERP.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddOnda1ErpPet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DiasValidadeAposAbertura",
                table: "Produtos",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "FatorConversaoGranel",
                table: "Produtos",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PermiteFracionamento",
                table: "Produtos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ProdutoOrigemGranelId",
                table: "Produtos",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PlanoLicenca",
                table: "Empresas",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "AberturasSacoGranel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProdutoSacoId = table.Column<int>(type: "integer", nullable: false),
                    ProdutoGranelId = table.Column<int>(type: "integer", nullable: false),
                    DepositoId = table.Column<int>(type: "integer", nullable: false),
                    LoteId = table.Column<int>(type: "integer", nullable: true),
                    QuantidadeSacos = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    PesoTotalGeradoKg = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    CustoUnitarioGranel = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    DataAbertura = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataValidadeAposAbertura = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
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
                    table.PrimaryKey("PK_AberturasSacoGranel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AberturasSacoGranel_Depositos_DepositoId",
                        column: x => x.DepositoId,
                        principalTable: "Depositos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AberturasSacoGranel_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AberturasSacoGranel_Lotes_LoteId",
                        column: x => x.LoteId,
                        principalTable: "Lotes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AberturasSacoGranel_Produtos_ProdutoGranelId",
                        column: x => x.ProdutoGranelId,
                        principalTable: "Produtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AberturasSacoGranel_Produtos_ProdutoSacoId",
                        column: x => x.ProdutoSacoId,
                        principalTable: "Produtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ConveniosPet",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Cnpj = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Telefone = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    PercentualCoberturaPadrao = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
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
                    table.PrimaryKey("PK_ConveniosPet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConveniosPet_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmpresaModulos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ModuloCodigo = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    Origem = table.Column<int>(type: "integer", nullable: false),
                    DataAtivacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataExpiracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CriadoPor = table.Column<string>(type: "text", nullable: true),
                    AlteradoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AlteradoPor = table.Column<string>(type: "text", nullable: true),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpresaModulos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmpresaModulos_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ModulosSistema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Codigo = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    Nome = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Descricao = table.Column<string>(type: "text", nullable: false),
                    Categoria = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Ordem = table.Column<int>(type: "integer", nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModulosSistema", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Especie = table.Column<int>(type: "integer", nullable: false),
                    Raca = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CorPelagem = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    Sexo = table.Column<int>(type: "integer", nullable: false),
                    Castrado = table.Column<bool>(type: "boolean", nullable: false),
                    DataNascimento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IdadeEstimadaMeses = table.Column<int>(type: "integer", nullable: true),
                    Porte = table.Column<int>(type: "integer", nullable: false),
                    TipoPelagem = table.Column<int>(type: "integer", nullable: false),
                    PesoAtualKg = table.Column<decimal>(type: "numeric(8,3)", precision: 8, scale: 3, nullable: true),
                    Microchip = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    FotoUrl = table.Column<string>(type: "text", nullable: true),
                    Temperamento = table.Column<int>(type: "integer", nullable: false),
                    Alergias = table.Column<string>(type: "text", nullable: true),
                    RestricoesAlimentares = table.Column<string>(type: "text", nullable: true),
                    Observacoes = table.Column<string>(type: "text", nullable: true),
                    ConvenioPetId = table.Column<int>(type: "integer", nullable: true),
                    NumeroCarteirinhaConvenio = table.Column<string>(type: "text", nullable: true),
                    Obito = table.Column<bool>(type: "boolean", nullable: false),
                    DataObito = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CausaObito = table.Column<string>(type: "text", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CriadoPor = table.Column<string>(type: "text", nullable: true),
                    AlteradoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AlteradoPor = table.Column<string>(type: "text", nullable: true),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pets_ConveniosPet_ConvenioPetId",
                        column: x => x.ConvenioPetId,
                        principalTable: "ConveniosPet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Pets_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ComandasAtendimento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Numero = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    ClienteId = table.Column<int>(type: "integer", nullable: false),
                    PetId = table.Column<int>(type: "integer", nullable: true),
                    ConvenioPetId = table.Column<int>(type: "integer", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    DataAbertura = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataFechamento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ValorBruto = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    ValorDesconto = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    ValorCobertoConvenio = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    ValorLiquido = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    VendaPdvId = table.Column<int>(type: "integer", nullable: true),
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
                    table.PrimaryKey("PK_ComandasAtendimento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComandasAtendimento_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ComandasAtendimento_ConveniosPet_ConvenioPetId",
                        column: x => x.ConvenioPetId,
                        principalTable: "ConveniosPet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ComandasAtendimento_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComandasAtendimento_Pets_PetId",
                        column: x => x.PetId,
                        principalTable: "Pets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ComandasAtendimento_VendasPdv_VendaPdvId",
                        column: x => x.VendaPdvId,
                        principalTable: "VendasPdv",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TutorPets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClienteId = table.Column<int>(type: "integer", nullable: false),
                    PetId = table.Column<int>(type: "integer", nullable: false),
                    TipoVinculo = table.Column<int>(type: "integer", nullable: false),
                    PodeAssinarTermos = table.Column<bool>(type: "boolean", nullable: false),
                    RecebeNotificacoes = table.Column<bool>(type: "boolean", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CriadoPor = table.Column<string>(type: "text", nullable: true),
                    AlteradoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AlteradoPor = table.Column<string>(type: "text", nullable: true),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TutorPets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TutorPets_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TutorPets_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TutorPets_Pets_PetId",
                        column: x => x.PetId,
                        principalTable: "Pets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ComandaItens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ComandaAtendimentoId = table.Column<int>(type: "integer", nullable: false),
                    Origem = table.Column<int>(type: "integer", nullable: false),
                    TipoItem = table.Column<int>(type: "integer", nullable: false),
                    ProdutoId = table.Column<int>(type: "integer", nullable: true),
                    ServicoId = table.Column<int>(type: "integer", nullable: true),
                    Descricao = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Quantidade = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    ValorUnitario = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    ValorDesconto = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    ValorCobertoConvenio = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    ValorTotal = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CobertoPorPacote = table.Column<bool>(type: "boolean", nullable: false),
                    DocumentoFiscalDestino = table.Column<int>(type: "integer", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CriadoPor = table.Column<string>(type: "text", nullable: true),
                    AlteradoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AlteradoPor = table.Column<string>(type: "text", nullable: true),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComandaItens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComandaItens_ComandasAtendimento_ComandaAtendimentoId",
                        column: x => x.ComandaAtendimentoId,
                        principalTable: "ComandasAtendimento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComandaItens_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComandaItens_Produtos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produtos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ComandaItens_Servicos_ServicoId",
                        column: x => x.ServicoId,
                        principalTable: "Servicos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_ProdutoOrigemGranelId",
                table: "Produtos",
                column: "ProdutoOrigemGranelId");

            migrationBuilder.CreateIndex(
                name: "IX_AberturasSacoGranel_DepositoId",
                table: "AberturasSacoGranel",
                column: "DepositoId");

            migrationBuilder.CreateIndex(
                name: "IX_AberturasSacoGranel_EmpresaId",
                table: "AberturasSacoGranel",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_AberturasSacoGranel_LoteId",
                table: "AberturasSacoGranel",
                column: "LoteId");

            migrationBuilder.CreateIndex(
                name: "IX_AberturasSacoGranel_ProdutoGranelId",
                table: "AberturasSacoGranel",
                column: "ProdutoGranelId");

            migrationBuilder.CreateIndex(
                name: "IX_AberturasSacoGranel_ProdutoSacoId",
                table: "AberturasSacoGranel",
                column: "ProdutoSacoId");

            migrationBuilder.CreateIndex(
                name: "IX_ComandaItens_ComandaAtendimentoId",
                table: "ComandaItens",
                column: "ComandaAtendimentoId");

            migrationBuilder.CreateIndex(
                name: "IX_ComandaItens_EmpresaId",
                table: "ComandaItens",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_ComandaItens_ProdutoId",
                table: "ComandaItens",
                column: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_ComandaItens_ServicoId",
                table: "ComandaItens",
                column: "ServicoId");

            migrationBuilder.CreateIndex(
                name: "IX_ComandasAtendimento_ClienteId",
                table: "ComandasAtendimento",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_ComandasAtendimento_ConvenioPetId",
                table: "ComandasAtendimento",
                column: "ConvenioPetId");

            migrationBuilder.CreateIndex(
                name: "IX_ComandasAtendimento_EmpresaId_Numero",
                table: "ComandasAtendimento",
                columns: new[] { "EmpresaId", "Numero" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ComandasAtendimento_PetId",
                table: "ComandasAtendimento",
                column: "PetId");

            migrationBuilder.CreateIndex(
                name: "IX_ComandasAtendimento_VendaPdvId",
                table: "ComandasAtendimento",
                column: "VendaPdvId");

            migrationBuilder.CreateIndex(
                name: "IX_ConveniosPet_EmpresaId",
                table: "ConveniosPet",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_EmpresaModulos_EmpresaId_ModuloCodigo",
                table: "EmpresaModulos",
                columns: new[] { "EmpresaId", "ModuloCodigo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ModulosSistema_Codigo",
                table: "ModulosSistema",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pets_ConvenioPetId",
                table: "Pets",
                column: "ConvenioPetId");

            migrationBuilder.CreateIndex(
                name: "IX_Pets_EmpresaId_Nome",
                table: "Pets",
                columns: new[] { "EmpresaId", "Nome" });

            migrationBuilder.CreateIndex(
                name: "IX_TutorPets_ClienteId",
                table: "TutorPets",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_TutorPets_EmpresaId",
                table: "TutorPets",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_TutorPets_PetId_ClienteId",
                table: "TutorPets",
                columns: new[] { "PetId", "ClienteId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_Produtos_ProdutoOrigemGranelId",
                table: "Produtos",
                column: "ProdutoOrigemGranelId",
                principalTable: "Produtos",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Produtos_Produtos_ProdutoOrigemGranelId",
                table: "Produtos");

            migrationBuilder.DropTable(
                name: "AberturasSacoGranel");

            migrationBuilder.DropTable(
                name: "ComandaItens");

            migrationBuilder.DropTable(
                name: "EmpresaModulos");

            migrationBuilder.DropTable(
                name: "ModulosSistema");

            migrationBuilder.DropTable(
                name: "TutorPets");

            migrationBuilder.DropTable(
                name: "ComandasAtendimento");

            migrationBuilder.DropTable(
                name: "Pets");

            migrationBuilder.DropTable(
                name: "ConveniosPet");

            migrationBuilder.DropIndex(
                name: "IX_Produtos_ProdutoOrigemGranelId",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "DiasValidadeAposAbertura",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "FatorConversaoGranel",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "PermiteFracionamento",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "ProdutoOrigemGranelId",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "PlanoLicenca",
                table: "Empresas");
        }
    }
}
