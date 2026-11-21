using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ERP.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddOnda5TutorPortalWhatsApp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AcessosPortalTutor",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClienteId = table.Column<int>(type: "integer", nullable: false),
                    TokenAcesso = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    ValidoAte = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UltimoAcessoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CriadoPor = table.Column<string>(type: "text", nullable: true),
                    AlteradoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AlteradoPor = table.Column<string>(type: "text", nullable: true),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcessosPortalTutor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AcessosPortalTutor_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AcessosPortalTutor_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConfiguracoesWhatsAppEmpresa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Provedor = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    BaseUrl = table.Column<string>(type: "text", nullable: true),
                    InstanceName = table.Column<string>(type: "text", nullable: true),
                    ApiKey = table.Column<string>(type: "text", nullable: true),
                    EnviarBanhoPronto = table.Column<bool>(type: "boolean", nullable: false),
                    EnviarLembreteVacina = table.Column<bool>(type: "boolean", nullable: false),
                    EnviarBoletimInternacao = table.Column<bool>(type: "boolean", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CriadoPor = table.Column<string>(type: "text", nullable: true),
                    AlteradoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AlteradoPor = table.Column<string>(type: "text", nullable: true),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracoesWhatsAppEmpresa", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConfiguracoesWhatsAppEmpresa_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NotificacoesWhatsAppOutbox",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClienteId = table.Column<int>(type: "integer", nullable: true),
                    PetId = table.Column<int>(type: "integer", nullable: true),
                    TelefoneDestino = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    TipoEvento = table.Column<int>(type: "integer", nullable: false),
                    Mensagem = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    DataAgendada = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataEnvio = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Tentativas = table.Column<int>(type: "integer", nullable: false),
                    ErroDetalhe = table.Column<string>(type: "text", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CriadoPor = table.Column<string>(type: "text", nullable: true),
                    AlteradoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AlteradoPor = table.Column<string>(type: "text", nullable: true),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificacoesWhatsAppOutbox", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificacoesWhatsAppOutbox_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_NotificacoesWhatsAppOutbox_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NotificacoesWhatsAppOutbox_Pets_PetId",
                        column: x => x.PetId,
                        principalTable: "Pets",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AcessosPortalTutor_ClienteId",
                table: "AcessosPortalTutor",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_AcessosPortalTutor_EmpresaId",
                table: "AcessosPortalTutor",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_AcessosPortalTutor_TokenAcesso",
                table: "AcessosPortalTutor",
                column: "TokenAcesso",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracoesWhatsAppEmpresa_EmpresaId",
                table: "ConfiguracoesWhatsAppEmpresa",
                column: "EmpresaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NotificacoesWhatsAppOutbox_ClienteId",
                table: "NotificacoesWhatsAppOutbox",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificacoesWhatsAppOutbox_EmpresaId_Status",
                table: "NotificacoesWhatsAppOutbox",
                columns: new[] { "EmpresaId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_NotificacoesWhatsAppOutbox_PetId",
                table: "NotificacoesWhatsAppOutbox",
                column: "PetId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AcessosPortalTutor");

            migrationBuilder.DropTable(
                name: "ConfiguracoesWhatsAppEmpresa");

            migrationBuilder.DropTable(
                name: "NotificacoesWhatsAppOutbox");
        }
    }
}
