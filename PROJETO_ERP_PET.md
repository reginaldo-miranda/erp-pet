# 🐾 Projeto ERP Pet — Especificação Arquitetural e Plano de Transformação

> Documento de especificação funcional, arquitetural e roadmap para transformação do **ERP Multi-Empresa** (`.NET 8 + Blazor Server + MudBlazor + PostgreSQL 16`) em um **ERP Pet SaaS Modular** para **Loja de Pet**, **Banho & Tosa** e **Hospital Veterinário**, atendendo clientes de **pequeno, médio e grande porte**.

---

## 1. Visão Geral e Modelo de Negócio

### 1.1 Posicionamento
- **Modelo:** Produto **SaaS Multi-Empresa** com módulos ativáveis por empresa (`EmpresaId`) e possibilidade de **add-ons** avulsos.
- **Implantação Híbrida:**
  - **Nuvem (SaaS Multi-Tenant):** Operação padrão para clientes pequenos, médios e redes.
  - **Local / On-Premise (Docker):** Suportada via `Dockerfile` e `docker-compose.yml` para hospitais 24h, clínicas com internet instável ou integração local direta com balanças, impressoras térmicas e equipamentos de laboratório.

### 1.2 Empacotamento por Porte

| Porte | Perfil de Cliente | Módulos Base Incluídos |
|---|---|---|
| **Pequeno** | Pet shop de bairro e/ou Banho & Tosa | Núcleo (`Tutor`/`Pet`), Loja/PDV (com Granel e Balança), Banho & Tosa, Comanda Unificada, Estoque, Financeiro, Fiscal (`NFC-e`/`NF-e`/`NFS-e`) |
| **Médio** | Pet shop + Clínica Veterinária diurna | Tudo do Pequeno + Ambulatório Veterinário (`SOAP`, Vacinas, Receituário), Pacotes/Comissões avançadas, WhatsApp + Portal do Tutor + Agendamento Online |
| **Grande** | Hospital Veterinário 24h / Rede Multi-Filial | Tudo do Médio + Internação 24h (Baias + Enfermagem), Centro Cirúrgico & Exames, Integração HL7/LIS, Convênios Pet, Multi-Filial e Centros de Custo |

> Todos os planos permitem ativar/desativar módulos individualmente (**Add-ons**) sem reinstalação.

---

## 2. Arquitetura Modular sobre o ERP Atual

### 2.1 Estratégia: Monolito Modular com Feature Flags por Empresa
Em vez de múltiplos binários ou bases separadas, o sistema mantém a Clean Architecture atual (`ERP.Domain`, `ERP.Application`, `ERP.Infrastructure`, `ERP.Web`) organizada por **módulos isolados**, controlada por licenciamento por empresa:

1. **Novas Entidades de Licenciamento (Core):**
   - `ModuloSistema` (catálogo de módulos: `LojaGranel`, `BanhoTosa`, `VetAmbulatorio`, `VetInternacao`, `VetCirurgico`, `VetLabHl7`, `PortalTutor`, `WhatsApp`, `ConveniosPet`).
   - `PlanoLicenca` (Small / Medium / Large / Custom).
   - `EmpresaModulo` (`EmpresaId`, `ModuloCodigo`, `Ativo`, `DataExpiracao`, `Origem: Plano | AddOn`).
2. **Enforcement em 3 Camadas:**
   - **UI (`NavMenu.razor` e páginas Blazor):** Oculta menus, abas e blocos da `Comanda Unificada` cujos módulos não estejam ativos para a `Empresa` atual.
   - **Application (`MediatR` Pipeline Behavior):** `ModuloAtivoBehavior<TRequest, TResponse>` valida atributos `[RequerModulo("...")]` nos Commands/Queries antes da execução.
   - **Autorização (`Permissao`):** Continua operando no formato `"Modulo.SubModulo.Acao"` (ex.: `"Vet.Internacao.Prescrever"`, `"BanhoTosa.Agenda.Gerenciar"`), somada à verificação de módulo ativo.

---

## 3. Núcleo Base Unificado (`Tutor`, `Pet` e `Comanda Unificada`)

### 3.1 Relacionamento Tutor ↔ Pet (`N:N`)
- Reaproveita `Cliente` (`ERP.Domain/Core/Entities/Cadastros/Cliente.cs`) como **Tutor**, adicionando dados específicos quando necessário (preferência de contato, opt-in WhatsApp).
- Nova entidade **`Pet`**:
  - Identificação: `Nome`, `Especie` (Cão, Gato, Silvestre/Exótico, etc.), `Raca`, `Sexo`, `Castrado`, `DataNascimento` (ou idade estimada), `Porte` (`P`, `M`, `G`, `GG`), `TipoPelagem` (`Curta`, `Longa`, `Dupla`, `Lisa`, `Encaracolada`), `PesoAtualKg`, `Microchip`, `FotoUrl`.
  - Saúde & Comportamento: `Temperamento` (Dócil, Medroso, Reativo/Agressivo), `Alergias`, `RestricoesAlimentares`, `ObservacoesGerais`, `Obito` (`DataObito`, `Causa`).
- Entidade associativa **`TutorPet`** (`N:N`):
  - `ClienteId` (Tutor), `PetId`, `TipoVinculo` (`Principal`, `Secundario`, `AutorizadoRetirada`), `PodeAssinarTermos`, `RecebeNotificacoes`.
  - **Regra:** Todo o histórico (banhos, consultas, vacinas, internações, exames) pertence ao **`Pet`**, garantindo continuidade mesmo se houver troca ou adição de tutores.

### 3.2 Comanda Unificada (Hub de Consumo)
Para unir Loja + Banho & Tosa + Hospital em um único atendimento ao cliente:
- Nova entidade **`ComandaAtendimento`**:
  - Agrupa itens originados de qualquer vertical para o mesmo `Tutor` / `Pet` durante uma visita ou período de internação.
  - Origens possíveis de `ComandaItem`:
    1. **Loja:** Produtos de balcão / ração a granel.
    2. **Banho & Tosa:** Serviços executados, adicionais ou consumo de pacote (valor R$ 0,00 no checkout quando coberto por saldo de pacote).
    3. **Hospital:** Consultas, diárias de internação, taxas cirúrgicas, exames e **insumos/medicamentos consumidos** na enfermagem.
  - Suporta **múltiplos meios de pagamento**, **split de pagamento** e **cobertura parcial/total por Convênio Pet** antes de encerrar no `PDV` (`VendaPdv`).

---

## 4. Análise Operacional dos 3 Verticais

### 4.1 Vertical 1 — Loja de Pet (Varejo, Granel & PDV)
**Dores reais do varejo pet:** venda de ração a granel a partir de sacos fechados, leitura de etiquetas de balança no PDV, controle de validade e vendas rápidas no caixa.

- **Fracionamento / Abertura de Saco para Granel:**
  - Reaproveita `Produto`, `UnidadeMedida` e `Estoque`.
  - Adiciona suporte a `ProdutoPaiId` / `FatorConversao` e operação **"Abrir Embalagem para Granel"**:
    - Ex.: dá baixa de `1 UN` do produto *Ração X 15kg* (custo R$ 180,00) e gera entrada automática de `15,000 KG` no produto *Ração X Granel (kg)* (custo unitário R$ 12,00/kg), preservando lote e data de validade do saco aberto (com alerta de validade pós-abertura).
- **PDV com Balança / Código de Barras (`ERP.Application/Pdv`):**
  - Parser de código de barras padrão de balança (`EAN-13` iniciado em `2`, extraindo código do produto + peso ou valor total).
  - Leitura direta de peso via integração local (quando usando agente/Docker local) ou digitação rápida no PDV.
- **Integração com Comanda:**
  - O caixa pode puxar uma `ComandaAtendimento` aberta (ex.: cliente veio buscar o pet no banho e pegou um petisco/ração na loja) e fechar tudo em uma única cobrança.

---

### 4.2 Vertical 2 — Banho & Tosa (Estética Pet)
**Dores reais do banho e tosa:** gargalo de agenda por profissional/banheira, divergência de preço por porte/pelagem, reclamações sobre nós/pulgas/feridas pré-existentes, controle de pacotes mensais e cálculo justo de comissão.

- **Tabela de Preços Dinâmica:**
  - Estende `Servico` com matriz de preço por **`Porte` × `TipoPelagem`** (ex.: Banho P Pelo Curto vs Banho G Pelo Longo) + lista de **Adicionais** (desembolo, hidratação, tosa na tesoura, corte de unha).
- **Agenda Visual & Capacidade:**
  - Grade por **Profissional** (banhista/tosador) e **Recurso/Box** (banheira/mesa), com duração estimada automática conforme porte/pelagem e bloqueios de horário.
- **Check-in com Checklist & Fotos:**
  - No recebimento do pet: registro obrigatório/configurável de **condição física** (nós, pulgas/carrapatos, lesões de pele, otite, comportamento), **pertences** (coleira, guia, roupinha) e **fotos de entrada/saída** (com assinatura/aceite opcional do tutor).
- **Fila em Tempo Real (Kanban / Painel da Equipe):**
  - Fluxo de status: `Agendado` → `Aguardando (Check-in)` → `Em Banho` → `Secagem / Tosa` → `Pronto para Retirada` → `Entregue`.
  - A mudança para `Pronto para Retirada` dispara notificação automática via WhatsApp para o Tutor (quando módulo ativo) e libera o lançamento na `ComandaAtendimento`.
- **Pacotes de Banho & Comissões:**
  - Estende `ContratoRecorrente` (`Contratos`) para **Pacotes de Sessões** (ex.: 4 banhos/mês + 1 tosa higiênica), controlando saldo de sessões, validade e baixa automática no check-out da OS de banho.
  - Estende `RegraComissao` / `ComissaoVenda` (`Comissoes`) para **comissão por etapa** (ex.: 20% para o Banhista + 30% para o Tosador, inclusive sobre valor proporcional de sessões de pacote).

---

### 4.3 Vertical 3 — Hospital Veterinário (Clínica, Internação, Cirurgia & Lab)
**Dores reais de um hospital veterinário:** rastreabilidade clínica completa por pet, controle de horários de medicação na internação (evitar esquecimento ou dose dupla), perda de faturamento por insumos gastos na internação não cobrados do cliente, e integração de laudos.

#### A. Ambulatório & Consultas
- **Agenda Médica:** Por veterinário e especialidade (clínica geral, dermatologia, ortopedia, etc.), com encaixes e retornos.
- **Triagem:** Peso atual (atualiza histórico de peso do Pet), temperatura, FC, FR, TPC, mucosas e queixa principal.
- **Prontuário Eletrônico Veterinário (PEP):**
  - Estrutura **SOAP** (*Subjetivo, Objetivo, Avaliação, Plano*), diagnósticos, anexos e bloqueio/assinatura do atendimento pelo CRMV do médico responsável.
- **Receituário / Prescrição:**
  - Geração de receita simples e receita de **controle especial** (impressão/PDF e envio para o Portal do Tutor).
- **Carteira de Vacinação & Vermifugação:**
  - Registro de aplicação com vínculo ao **lote/validade do estoque**, dose e **cálculo automático da próxima dose/reforço** (alimentando alertas de retorno/WhatsApp).

#### B. Internação 24h & UTI
- **Mapa Visual de Baias / Leitos:**
  - Setores configuráveis (Canil, Gatil, Isolamento/Infectocontagiosas, UTI) com status (`Livre`, `Ocupada`, `Em Higienização`, `Manutenção`).
- **Prescrição & Aprazamento de Enfermagem:**
  - Grade horária gerada automaticamente a partir da prescrição do veterinário (ex.: `08h - 16h - 00h`), fluidoterapia e dieta.
  - **Checagem (Check-off) pela Enfermagem:** Ao marcar a medicação/insumo como *Aplicado*, o sistema:
    1. Registra quem aplicou e a hora real;
    2. **Debita o medicamento/insumo do `Estoque`** (incluindo baixa fracionada por `ml`, `comprimido`, `ampola`);
    3. **Lança automaticamente o item na `ComandaAtendimento`** da internação (zerando perdas de faturamento).
- **Evolução por Plantão & Sinais Vitais:** Linha do tempo contínua acessível na troca de plantão e resumo filtrável para o **Boletim Diário do Tutor**.

#### C. Centro Cirúrgico, Exames & Termos
- **Bloco Cirúrgico:** Agenda de sala cirúrgica, equipe (cirurgião, auxiliar, anestesista), **Ficha Anestésica** e consumo de kit cirúrgico vinculado ao estoque/comanda.
- **Termos de Consentimento:** Modelos parametrizáveis (internação, anestesia, cirurgia, transfusão, eutanásia) com aceite/assinatura digital do tutor autorizado no balcão ou Portal.
- **Exames & Integração HL7/LIS (v1):**
  - Solicitação de exames internos/externos, upload de imagens/laudos (Raio-X, Ultrassom, Lab).
  - Camada de integração via **HL7 / API LIS / importação de arquivos** para recepção automática de resultados de analisadores hematológicos/bioquímicos e laboratórios parceiros direto no prontuário do `Pet`.

---

## 5. Comunicação, Portal do Tutor, Fiscal e Convênios

### 5.1 Experiência do Tutor
- **Notificações WhatsApp:** Motor baseado em eventos (`MediatR` + fila/outbox) para disparos automáticos:
  - Confirmação/lembrete de agendamento;
  - *"Pet pronto para retirada"* no Banho & Tosa;
  - Alerta de reforço de vacina/vermífugo;
  - Boletim diário de internação e link para assinatura de termos.
- **Portal Web do Tutor:** Acesso via link seguro / login para consultar carteira de vacinas, receitas, laudos de exames, fotos do banho, assinar termos pendentes e realizar **Agendamento Online** (integrado à disponibilidade real de Banho & Tosa e Consultas).

### 5.2 Checkout Unificado, Split Fiscal e Convênios
- **Planos de Saúde Pet (Convênios):**
  - Cadastro de `ConvenioPet` (operadora, regras de coparticipação/cobertura por procedimento, número da carteirinha no `Pet`) abatendo ou faturando contra a operadora no fechamento da `ComandaAtendimento`.
- **Split Fiscal Automático (`ERP.Application/Fiscal`):**
  - Ao faturar uma `ComandaAtendimento` mista no PDV:
    - Itens do tipo **Produto** (rações, acessórios, medicamentos vendidos) → emitem **NFC-e** (ou **NF-e**).
    - Itens do tipo **Serviço** (banho, tosa, consulta, internação, cirurgia) → emitem **NFS-e**.

---

## 6. Roadmap de Implementação (5 Ondas)

| Onda | Foco | Principais Entregáveis |
|---|---|---|
| **Onda 1** | **Fundação Modular, Núcleo Pet & Varejo Granel** | `ModuloSistema` / `EmpresaModulo` + `ModuloAtivoBehavior` + `NavMenu` dinâmico; `Pet` + `TutorPet` (`N:N`); Fracionamento/Granel + parser de balança no PDV; `ComandaAtendimento` + Split Fiscal (`NFC-e/NF-e` vs `NFS-e`) + base de `ConvenioPet`. |
| **Onda 2** | **Banho & Tosa Completo** | Matriz de preço `Porte × Pelagem`; Agenda visual por profissional/box; Check-in (checklist + fotos); Fila em tempo real (Kanban); Pacotes de sessões (`Contratos`) e comissão por etapa (`Comissoes`). |
| **Onda 3** | **Hospital — Ambulatório & Vacinas** | Agenda médica; Triagem; Prontuário Eletrônico `SOAP`; Receituário (simples/especial); Carteira de vacinas e vermífugos com baixa de lote e alertas de reforço. |
| **Onda 4** | **Hospital — Internação 24h, Cirurgia & Lab** | Mapa de Baias/Leitos; Prescrição aprazada + checagem de enfermagem com baixa automática no `Estoque` e lançamento na `Comanda`; Evolução por plantão; Bloco cirúrgico + Ficha anestésica; Termos assináveis; Integração de exames HL7/LIS. |
| **Onda 5** | **Experiência Externa do Tutor** | Motor de notificações WhatsApp (eventos de agenda, banho pronto, vacinas, boletim); Portal Web do Tutor; Agendamento Online self-service. |

---

## 7. Mapeamento de Reaproveitamento do Código Existente

- `ERP.Domain/Core/Entities/Empresa.cs` → Base do Multi-Tenancy (`EmpresaModulo`).
- `ERP.Domain/Core/Entities/Permissao.cs` → Base do RBAC por `"Modulo.SubModulo.Acao"`.
- `ERP.Domain/Core/Entities/Cadastros/Cliente.cs` → Base do cadastro de `Tutor`.
- `ERP.Domain/Core/Entities/Cadastros/Produto.cs` & `Estoque/` → Base para fracionamento a granel e baixa fracionada hospitalar.
- `ERP.Domain/Core/Entities/Servicos/OrdemServico.cs` → Base conceitual/estrutural para ordens de Banho & Tosa.
- `ERP.Domain/Core/Entities/Contratos/ContratoRecorrente.cs` → Base para Pacotes de Banho & Tosa.
- `ERP.Domain/Core/Entities/Comissoes/` → Base para comissionamento de banhistas, tosadores e veterinários.
- `ERP.Domain/Core/Entities/Pdv/` & `ERP.Application/Fiscal/` → Base para checkout da `ComandaAtendimento` com split fiscal.
