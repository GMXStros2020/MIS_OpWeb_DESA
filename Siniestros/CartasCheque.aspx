<%@ Page Title="" Language="VB" MasterPageFile="~/Pages/SiteMaster.master" AutoEventWireup="false" CodeFile="CartasCheque.aspx.vb" Inherits="Siniestros_CartasCheque" %>

<%@ MasterType VirtualPath="~/Pages/SiteMaster.master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="cph_principal" runat="Server">
    <script src="../Scripts/Siniestros/CartasCheque.js"></script>
    <div class="zona-principal" style="overflow-x: hidden; overflow-y: hidden">

        <div class="cuadro-titulo panel-encabezado">
            <input type="image" src="../Images/contraer_mini_inv.png" id="coVentana0" class="contraer" />
            <input type="image" src="../Images/expander_mini_inv.png" id="exVentana0" class="expandir" />
            Filtros - Cartas Cheque
        </div>

        <div class="panel-contenido ventana0">
            <asp:UpdatePanel runat="server" ID="upFiltros">
                <ContentTemplate>
                    <asp:HiddenField runat="server" ID="hid_Ventanas" Value="0|1" />
                    <div class="padding10"></div>
                    <div class="row">
                        <div class="col-md-6">

                            <asp:Label runat="server" class="col-md-1 etiqueta-control" Width="20%">Num OP</asp:Label>
                            <asp:TextBox runat="server" ID="txt_nro_op" CssClass="col-md-1 estandar-control" Width="36%" onkeypress="return soloNumeros(event)" PlaceHolder="Ej: 84162"></asp:TextBox>

                            <asp:Label runat="server" class="col-md-1 etiqueta-control">A</asp:Label>
                            <asp:TextBox runat="server" ID="txt_nro_op_hasta" CssClass="col-md-1 estandar-control" Width="35.5%" onkeypress="return soloNumeros(event)" PlaceHolder="Núm OP hasta"></asp:TextBox>
                        </div>

                        <div class="col-md-6">

                            <asp:Label runat="server" class="col-md-1 etiqueta-control" Width="25%">Num Cheque</asp:Label>
                            <asp:TextBox runat="server" ID="txt_nro_ch_desde" CssClass="col-md-1 estandar-control" Width="33.2%" onkeypress="return soloNumeros(event)" PlaceHolder="Núm cheque desde"></asp:TextBox>

                            <asp:Label runat="server" class="col-md-1 etiqueta-control">A</asp:Label>
                            <asp:TextBox runat="server" ID="txt_nro_ch_hasta" CssClass="col-md-1 estandar-control" Width="33.2%" onkeypress="return soloNumeros(event)" PlaceHolder="Núm cheque hasta"></asp:TextBox>
                        </div>
                    </div>

                    <div class="clear padding5"></div>

                    <div class="row">
                        <div class="col-md-6">

                            <asp:Label runat="server" class="col-md-2 etiqueta-control" Width="20%">Siniestro</asp:Label>
                            <asp:TextBox runat="server" ID="txt_nro_stro" CssClass="col-md-1 estandar-control" PlaceHolder="Ej: 201308131" Width="80%" onkeypress="return soloNumeros(event)"></asp:TextBox>
                        </div>

                        <div class="col-md-6">

                            <asp:Label runat="server" class="col-md-1 etiqueta-control " Width="25%">Beneficiario</asp:Label>
                            <asp:TextBox runat="server" ID="txt_cheque_a_nom" CssClass="col-md-1 estandar-control" Width="75%" PlaceHolder="Ej: banco" OnFocusOut="convMayusculas('txt_cheque_a_nom')"></asp:TextBox>
                        </div>
                    </div>

                    <div class="padding5"></div>                

                    <div class="row cuadro-subtitulo"  style="text-align: center">
                        <div class="col-md-3"></div>
                        <div class="col-md-6" style="text-align: center">
                                <table style="width:100%; text-align:center">
                                    <tr>
                                        <td></td>
                                        <td><asp:label runat="server" class="etiqueta-control">Estatus:</asp:label></td>                                                                            
                                        
                                        <td><asp:RadioButton runat="server" ID="chk_Todas" Text="&nbspTodas" CssClass="etiqueta-control" Width="100px" AutoPostBack="true" OnCheckedChanged="chk_Todas_CheckedChanged"/></td>
                                        <td><asp:RadioButton runat="server" ID="chk_Pendientes" Text="&nbspPendientes" CssClass="etiqueta-control" Width="160px" AutoPostBack="true" OnCheckedChanged="chk_Pendientes_CheckedChanged" /></td>                                      
                                        <td><asp:RadioButton runat="server" ID="chk_Elaboradas"  Text="&nbspElaboradas" CssClass="etiqueta-control" Width="100px" AutoPostBack="true" OnCheckedChanged="chk_Elaboradas_CheckedChanged" /></td>                                                                               
                                        
                                    </tr>
                                </table>
                                
                            </div>
                        <div class="col-md-3"></div>
                    </div>

                    <div class="padding5"></div>                

                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>

    <div style="width: 100%; text-align: right; ">
        <div class="padding10">
            <asp:UpdatePanel runat="server" ID="upBusqueda">
                <ContentTemplate>

                    <asp:LinkButton ID="btn_BuscarOP" runat="server" class="btn botones" BorderWidth="2" BorderColor="White" Width="105px">
                        <span>
                            <img class="btn-buscar"/>&nbsp Buscar
                        </span>
                    </asp:LinkButton>

                      

                    <asp:LinkButton ID="btnLimpiar" runat="server" class="btn botones" BorderWidth="2" BorderColor="White" Width="130px">
                        <span>
                            <img class="btn-limpiar"/>&nbsp&nbsp Limpiar Filtros
                        </span>
                    </asp:LinkButton>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>

    <div class="padding20">

        <asp:UpdatePanel runat="server" ID="updGrd" UpdateMode="Conditional" Width="25%">
            <ContentTemplate>
                <asp:Panel runat="server" ID="pnlUsuario" Width="99%" Height="300" ScrollBars="Auto">
                    <asp:GridView ID="grd" runat="server" AutoGenerateColumns="false" CssClass="table grid-view " HeaderStyle-CssClass="header" AlternatingRowStyle-CssClass="altern" GridLines="Vertical" ShowHeaderWhenEmpty="true" ShowHeader="True" Width="95%" RowStyle-VerticalAlign="Middle">
                        <Columns>
                            <asp:BoundField DataField="nro_stro" HeaderText="SINIESTRO" />
                            <asp:BoundField DataField="nro_ch" HeaderText="CHEQUE" />
                            <asp:BoundField DataField="nro_op" HeaderText="ORDEN DE PAGO" />
                            <asp:BoundField DataField="txt_cheque_a_nom" HeaderText="BENEFICIARIO" />
                            <asp:BoundField DataField="imp_total" HeaderText="MONTO" DataFormatString="{0:N2}">
                                <ItemStyle HorizontalAlign="right"/>
                            </asp:BoundField>


                            <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:ImageButton ImageUrl="~/Images/delete_rojo.png" CommandName="Delete" runat="server" CssClass="btn Delete" Height="25" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>

                    </asp:GridView>
                    <div style="width: 100%; text-align: right;" class="padding10"></div>
                </asp:Panel>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="btn_BuscarOP" EventName="Click" />
            </Triggers>
        </asp:UpdatePanel>
    </div>



    <div class="padding20">
        <asp:UpdatePanel runat="server" ID="upPanelGenCartas" Width="99%" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:Panel runat="server" ID="PanelGenCartas" HorizontalAlign="Right">

                    <asp:LinkButton ID="btn_Continuar" runat="server" class="btn botones" Width="153px" data-toggle="modal" data-target="#modGeneraCartas" Visible="false">
                        <span>
                            <img class="btn-aceptar"/>&nbspContinuar
                        </span>
                    </asp:LinkButton>
                </asp:Panel>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="btn_BuscarOP" EventName="Click" />
            </Triggers>
        </asp:UpdatePanel>
    </div>

     <div class="padding53"></div>

    <!-- M O D A L  -->
    <div id="modGeneraCartas" style="width: 50%; left: 600px;" class="modal-catalogo fade">
        <asp:UpdatePanel runat="server" ID="upDatosEntrega" UpdateMode="Always">
            <ContentTemplate>
                <asp:Panel runat="server" ID="panDatosEntrega">
                    <div class="cuadro-titulo" style="height: 30px">

                        <div class="titulo-modal">DATOS PARA LA ENTREGA DE CHEQUES</div>
                    </div>

                    <div class="panel-contenido Centrado">
                        <div class="row align-items-center">

                            <div class="col-md-3" style="text-align: center">
                                <asp:CheckBox runat="server" ID="chkEnvio" CssClass="etiqueta-control" AutoPostBack="true" Text="&nbsp&nbspEnvío" />
                            </div>

                            <div class="col-md-9" style="text-align: center">

                                <div class="row align-items-center padding10">
                                    <asp:Label runat="server" class="col-md-3 etiqueta-control text-right padding10" Width="35%">Clave </asp:Label>
                                    <asp:TextBox runat="server" ID="txt_cc_Clave" CssClass="col-md-9 estandar-control" PlaceHolder="Ejemplo: 1, 30, 540" Width="57%" onkeypress="return soloNumeros(event)" Enabled="false" AutoPostBack="true" OnTextChanged="txt_cc_Clave_TextChanged"></asp:TextBox>
                                </div>


                                <div class="row align-items-center padding10">
                                    <asp:Label runat="server" class="col-md-3 etiqueta-control text-right padding10" Width="35%">Empresa </asp:Label>
                                    <asp:TextBox runat="server" ID="txtEmpresa" CssClass="col-md-9 estandar-control" Width="57%" Enabled="false"></asp:TextBox>
                                </div>


                                <div class="row align-items-center padding10">
                                    <asp:Label runat="server" class="col-md-3 etiqueta-control text-right padding10" Width="35%">Calle </asp:Label>
                                    <asp:TextBox runat="server" ID="txtCalle" CssClass="col-md-9 estandar-control" Width="57%" Enabled="false"></asp:TextBox>
                                </div>


                                <div class="row align-items-center padding10">
                                    <asp:Label runat="server" class="col-md-3 etiqueta-control text-right padding10" Width="35%">Colonia </asp:Label>
                                    <asp:TextBox runat="server" ID="txtColonia" CssClass="col-md-9 estandar-control" Width="57%" Enabled="false"></asp:TextBox>
                                </div>


                                <div class="row align-items-center padding10">
                                    <asp:Label runat="server" class="col-md-3 etiqueta-control text-right padding10" Width="35%">Delegación </asp:Label>
                                    <asp:TextBox runat="server" ID="txtDeleg" CssClass="col-md-9 estandar-control " Width="57%" Enabled="false"></asp:TextBox>
                                </div>


                                <div class="row align-items-center padding10">
                                    <asp:Label runat="server" class="col-md-3 etiqueta-control text-right padding10" Width="35%">Entidad </asp:Label>
                                    <asp:TextBox runat="server" ID="txtEntidad" CssClass="col-md-9 estandar-control" Width="57%" Enabled="false"></asp:TextBox>
                                </div>


                                <div class="row align-items-center padding10">
                                    <asp:Label runat="server" class="col-md-3 etiqueta-control text-right padding10" Width="35%">C.P. </asp:Label>
                                    <asp:TextBox runat="server" ID="txtCodPostal" CssClass="col-md-9 estandar-control" Width="57%" Enabled="false"></asp:TextBox>
                                </div>


                                <div class="row align-items-center padding10">
                                    <asp:Label runat="server" class="col-md-3 etiqueta-control text-right padding10" Width="35%">Atención </asp:Label>
                                    <asp:TextBox runat="server" ID="txtAtencion" CssClass="col-md-9 estandar-control" Width="57%" Enabled="false"></asp:TextBox>
                                </div>
                            </div>
                        </div>

                        <div class="row align-items-center padding20">
                            <div class="col-md-3"></div>
                            <div class="col-md-9">
                                <div class="col-md-1"></div>
                                <div class="col-md-10" style="text-align: center; border-top-style: inset; border-width: 1px; border-color: #003A5D; right: 1px;"></div>
                                <div class="col-md-1"></div>
                            </div>
                        </div>


                        <div class="row align-items-center padding10">

                            <div class="col-md-3" style="text-align: center">
                                <asp:CheckBox runat="server" ID="chkPresente" CssClass="etiqueta-control" AutoPostBack="true" Text="&nbsp&nbspPresente" />
                            </div>

                            <div class="col-md-9" style="text-align: center">

                                <div class="row align-items-center padding10">
                                    <asp:Label runat="server" class="col-md-4 etiqueta-control text-right padding10" Width="40%">Persona que recibe el cheque </asp:Label>
                                    <asp:TextBox runat="server" ID="txtRecibe" CssClass="col-md-9 estandar-control" Width="52%" Enabled="false" OnFocusOut="convMayusculas('txtRecibe')"></asp:TextBox>
                                </div>


                                <div class="row align-items-center padding10">
                                    <asp:Label runat="server" class="col-md-3 etiqueta-control text-right padding10" Width="35%">Empresa de donde viene </asp:Label>
                                    <asp:TextBox runat="server" ID="txtEmpresaRemite" CssClass="col-md-9 estandar-control" Width="57%" Enabled="false" OnFocusOut="convMayusculas('txtEmpresaRemite')"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div style="width: 100%; text-align: right; border-top-style: inset; border-width: 1px; padding: 10px; border-color: #003A5D;">
                        <asp:HiddenField runat="server" ID="hid_Folio" Value="0" />

                        <div class="row" style="width: 100%;">
                            <div class="col-md-4" style="text-align: right;">
                                <asp:CheckBox runat="server" ID="chk_autoriza" CssClass="etiqueta-control" AutoPostBack="true" Text="&nbsp&nbspAutorización Subdirector" />
                                </div>

                            <div class="col-md-8" style="text-align: right;">

                                <asp:LinkButton ID="btnPreview" runat="server" class="btn botones" Width="120px">
                                    <span>
                                        <img class="btn-buscar"/>&nbspRevisar Carta
                                    </span>
                                </asp:LinkButton>
                                <asp:Button ID="btnSolAut" runat="server" class="botones" Width="153px" Enabled="False" Text="Solicitar Autorización" BorderStyle="solid" BorderColor="#003A5D" Style="border-radius: 4px; height: 33px" />

                                <asp:LinkButton ID="btnCancela" runat="server" data-dismiss="modal" class="btn botones CierraFirma" Width="120px">
                                    <span>
                                        <img class="btn-cancelar"/>&nbspCancelar
                                    </span>
                                </asp:LinkButton>
                            </div>
                        </div>
                    </div>
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>

    <div id="modProcesado" class="modal-catalogo">
        <div class="cuadro-titulo-flotante">
            <button type="button" data-dismiss="modal" class="close" hidden="hidden">&times;</button>
            <div>
                <label id="lbl_Catalogo">Generacion de Cartas Cheque</label>
            </div>
        </div>
        <div class="clear padding5"></div>
        <asp:UpdatePanel runat="server" ID="upCatalogo">
            <ContentTemplate>
                <div class="input-group">
                    <br />
                    <asp:Label runat="server" class="col-md-12 etiqueta-control">Generación y Solicitud de Autorización de Carta cheque exitoso.
                    </asp:Label>
                    <asp:Label runat="server" class="col-md-12 etiqueta-control" ID="lblFolio" Text="" />

                    <br />
                    <br />
                </div>
                <div style="width: 100%; text-align: right;">
                    <asp:Button runat="server" ID="btnProcesado" class="btn botones" Text="Aceptar" />
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>


    <br />
    <br />
    <br />
    <br />



</asp:Content>

