<%@ Page Title="" Language="VB" MasterPageFile="~/Pages/SiteMaster.master" AutoEventWireup="false" CodeFile="AutElectFondos.aspx.vb" Inherits="Siniestros_AutElectFondos" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>
<%@ MasterType VirtualPath="~/Pages/SiteMaster.master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="cph_principal" Runat="Server">
    <script src="../Scripts/Siniestros/FirmasElectronicas.js"></script>

    <script type="text/javascript"> 
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(PageLoadFirmas);
        Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(PageLoadFirmas);
    </script> 

    
    
    <div class="zona-principal" style="overflow-x:hidden;overflow-y:hidden">
        <div class="cuadro-titulo panel-encabezado">
            <input type="image" src="../Images/contraer_mini_inv.png" id="coVentana0" class="contraer"  />
            <input type="image" src="../Images/expander_mini_inv.png"   id="exVentana0" class="expandir"  />
            Filtros - Autorizaciones Electrónicas Fondos
        </div>

        <div class="panel-contenido ventana0" >
            <asp:UpdatePanel runat="server" ID="upFiltros">
                <ContentTemplate>
                    <asp:HiddenField runat="server" ID="hid_Ventanas" Value="0|1|1|1|1|1|1|1|" />

                    <div class="row">
                        <div class="col-md-6">
                            <asp:label runat="server" class="col-md-1 etiqueta-control" Width="20%">Orden Pago</asp:label>
                            <asp:TextBox runat="server" ID="txt_NroOP" CssClass="col-md-1 estandar-control" PlaceHolder="Ejemplo: 84162" Width="80%"></asp:TextBox>
                            <asp:DropDownList runat="server" ID="cmbModuloOP" CssClass="col-md-1 estandar-control" Width="36%" Visible="false">
                                <asp:ListItem Text="Seleccione módulo" Value="0"></asp:ListItem>
                                <asp:ListItem Text="OP Tradicional" Value="1"></asp:ListItem>
                                <asp:ListItem Text="OP Fondos" Value="3"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-6">
                            <asp:label runat="server" class="col-md-1 etiqueta-control" Width="25%">Beneficiario</asp:label>
                            <asp:HiddenField runat="server" ID="hidClaveAse" Value="" />
                            <asp:textbox runat="server" ID="txtAsegurado" CssClass="estandar-control" Width="75%" ></asp:textbox>
                        </div>
                    </div>

                    <div class="clear padding5"></div>

                    <div class="row">
                        <div class="col-md-6">
                            <asp:label runat="server" class="col-md-1 etiqueta-control" Width="20%">Moneda</asp:label>
                            <asp:DropDownList runat="server" ID="cmbMoneda" CssClass="estandar-control" Width="80%" ></asp:DropDownList>
                        </div>
                        <div class="col-md-6">
                            <asp:label runat="server" class="col-md-1 etiqueta-control" Width="25%">Fecha Genera</asp:label>
                            <asp:TextBox runat="server" ID="txtFechaGeneracionDesde" CssClass="col-md-1 estandar-control Fecha Centro" Width="33.2%" ></asp:TextBox>
                            <asp:label runat="server" class="col-md-1 etiqueta-control">A</asp:label>
                            <asp:TextBox runat="server" ID="txtFechaGeneracionHasta" CssClass="estandar-control Fecha Centro" Width="33.2%" ></asp:TextBox>
                        </div>
                    </div>

                    <div class="clear padding5"></div>

                    <div class="row">
                        <div class="col-md-6">
                            <asp:label runat="server" class="col-md-1 etiqueta-control" Width="20%">Monto</asp:label>
                            <asp:TextBox runat="server" ID="txtMontoDesde" CssClass="col-md-1 estandar-control Monto Derecha" Width="36%" ></asp:TextBox>
                            <asp:label runat="server" class="col-md-1 etiqueta-control">A</asp:label>
                            <asp:TextBox runat="server" ID="txtMontoHasta" CssClass="estandar-control Monto Derecha" Width="35.5%" ></asp:TextBox>
                        </div>

                        <div class="col-md-6">
                            <asp:label runat="server" class="col-md-1 etiqueta-control" Width="25%">Fecha Pago</asp:label>
                            <asp:TextBox runat="server" ID="txtFechaPagoDesde" CssClass="col-md-1 estandar-control Fecha Centro" Width="33.2%" ></asp:TextBox>
                            <asp:label runat="server" class="col-md-1 etiqueta-control">A</asp:label>
                            <asp:TextBox runat="server" ID="txtFechaPagoHasta" CssClass="estandar-control Fecha Centro" Width="33.2%" ></asp:TextBox>
                        </div>

                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>

            <div class="clear padding5"></div>
             <div class="clear padding5"></div>

            <div class="row">
                <asp:UpdatePanel ID="up_Firmas" runat="server">
                    <ContentTemplate>
                        <div class="col-md-12">
                            <div class="cuadro-subtitulo">

                                <table style="width:80%">
                                    <tr>
                                        <td><asp:label runat="server" class="etiqueta-control">Estatus Firma Electrónica:</asp:label></td>
                                        <td><asp:RadioButton runat="server" ID="chk_Todas" Text="Todas" CssClass="etiqueta-control" Width="80px" GroupName="FiltrosG" Visible="false" /></td>
                                        <td><asp:RadioButton runat="server" ID="chk_MisPend" Text="Mis Pendientes" CssClass="etiqueta-control" Width="120px"  GroupName="FiltrosG" Visible="false"  /></td>
                                        <td><asp:RadioButton runat="server" ID="chk_SinFirma" Text="Sin Firma Previa" CssClass="etiqueta-control" Width="120px" GroupName="FiltrosG" Visible="false"  /></td>
                                        <td><asp:RadioButton runat="server" ID="chk_Rechazadas" Text="Cancelada/Sol.Rechazo" CssClass="etiqueta-control"  Width="150px" GroupName="FiltrosG" Visible="false"  /></td>
                                        <td><asp:RadioButton runat="server" ID="chk_FinalAut"  Text="Autorizadas" CssClass="etiqueta-control" Width="100px" GroupName="FiltrosG" Visible="false"  /></td>
                                        <%--<td><asp:RadioButton runat="server" ID="chk_NoProc"  Text="Sol.Rec" CssClass="etiqueta-control" Width="100px" AutoPostBack="true" Visible="false" /></td>--%>
                                        <asp:Label id="lblFechaTope" Text="Fecha Tope:" class="etiqueta-control" Font-Bold="true"  BackColor="LightGray" runat="server" ></asp:Label>
                                    </tr>
                                </table>
                                
                            </div>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>

            <div class="clear padding5"></div>
             <div class="clear padding5"></div>

         </div>
        <div style="width:100%; text-align:right; border-top-style:inset; border-width:1px; border-color:#003A5D">
            <asp:UpdatePanel runat="server" ID="upBusqueda">
                <ContentTemplate>
                    <asp:LinkButton id="btn_BuscaOP" runat="server" class="btn botones">
                        <span>
                            <img class="btn-buscar"/>
                            Buscar
                        </span>
                    </asp:LinkButton>

                    <asp:LinkButton id="btn_Limpiar" runat="server" class="btn botones">
                        <span>
                            <img class="btn-cancelar"/>
                            Cancelar
                        </span>
                    </asp:LinkButton>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
       
        <div class="clear padding5"></div>

        <div class="cuadro-titulo panel-encabezado">
            <input type="image" src="../Images/contraer_mini_inv.png" id="coVentana1" class="contraer"  />
            <input type="image" src="../Images/expander_mini_inv.png"   id="exVentana1" class="expandir"  />
            Listado Ordenes de Pago
        </div>

        <div class="panel-contenido ventana1" >
                <asp:UpdatePanel runat="server" ID="upOrdenes" >
                  <ContentTemplate>
                      <asp:Label Text="Firmadas" class="etiqueta-control" Font-Bold="true" BackColor="#66ff33" runat="server" ></asp:Label>
                      <asp:Label Text="Sin Firmar" class="etiqueta-control" Font-Bold="true"  BackColor="#ffcc00" runat="server" ></asp:Label>
                      <asp:Label Text="Sol.Rechazo" class="etiqueta-control" Font-Bold="true" BackColor="Red" runat="server" ForeColor="White" ></asp:Label>
                      <asp:Label Text="Cancelada" class="etiqueta-control" Font-Bold="true"  BackColor="#6699ff" runat="server" ></asp:Label>
                      <%--<asp:Label id="lblFechaTope" Text="Fecha Tope:" class="etiqueta-control" Font-Bold="true"  BackColor="LightGray" runat="server" ></asp:Label>--%>

                      <asp:HiddenField runat="server" ID="hid_rechazo" Value="0" />
                         <asp:Panel runat="server" id="pnlOrdenP" width="100%" Height="300px" ScrollBars="Vertical">
                             <asp:GridView runat="server" ID="grdOrdenPago" Width="100%" AutoGenerateColumns="false"  ShowHeader="True"
                                 CssClass="grid-view" HeaderStyle-CssClass="header" AlternatingRowStyle-CssClass="altern"
                                 GridLines="None"  ShowHeaderWhenEmpty="true"
                                 DataKeyNames="FolioOnbase, nro_op,	FechaGeneracion,	FechaBaja,	NumeroRecibo,	NombreSucursal,	NombreSucursalPago,	CodigoAbona,	NombreModifica,	NombreUsuario,
                                               txt_cheque_a_nom,	FechaEstimadaPago,	imp_total,	Observaciones,	NombreAbona,	Direccion,	Calle,	NumeroExterior,	NumeroInterior,
                                               Colonia,	CodigoPostal,	Municipio,	Ciudad,	Departamento,	Sector,	Transferencia,	CodigoBanco,	NombreBanco,	Swift,	Aba,	NumeroCuenta,
                                               Moneda,	txt_moneda,	CodigoAnulacion,	Concepto,	ClaveProveedor,	TipoTransferencia,	NumeroPoliza,	Contratante,	SubRamoContable,	NumeroSiniestro,
                                               ClasePago, Solicitante, Jefe, Tesoreria, Subdirector, Director, DirectorGeneral, Subgerente, NombreSolicitante,	NombreJefe, NombreTesoreria, 
                                               NombreSubdirector, NombreDirector, NombreDirectorGeneral, NombreSubgerente,	FirmaSolicitante, FirmaJefe, FirmaTesoreria, FirmaSubdirector, FirmaDirector,			
                                               FirmaDirectorGeneral, FirmaSubgerente, FirmadoSolicitante, FirmadoJefe, FirmadoTesoreria, FirmadoSubdirector, FirmadoDirector, FirmadoDirectorGeneral,FirmadoSubgerente,	FechaFirmaSolicitante,
                                               FechaFirmaJefe, FechaFirmaTesoreria ,FechaFirmaSubdirector ,FechaFirmaDirector ,FechaFirmaDirectorGeneral,FechaFirmaSubgerente, NivelAutorizacion, Preautorizada,Rechazada">

                                 <Columns>
                                      <asp:TemplateField HeaderText="OP">
                                            <ItemTemplate>
                                                    <asp:imagebutton ID="btn_VerPDF" ImageUrl="~/Images/pdf14.png" Height="26" CommandName="VerOP" CommandArgument="<%# Container.DataItemIndex %>" runat="server"/>
                                            </ItemTemplate>
                                    </asp:TemplateField>
                                      <asp:TemplateField HeaderText="EdoCta">
                                            <ItemTemplate>
                                                    <asp:imagebutton ID="btn_VerEdoCta" ImageUrl="~/Images/pdf14.png" Height="26" CommandName="VerEdoCta" CommandArgument="<%# Container.DataItemIndex %>" runat="server"/>
                                            </ItemTemplate>
                                    </asp:TemplateField>
                                      <asp:TemplateField HeaderText="Docs">
                                            <ItemTemplate>
                                                    <asp:imagebutton ID="btn_VerDocs" ImageUrl="~/Images/pdf14.png" Height="26" CommandName="VerDocs" CommandArgument="<%# Container.DataItemIndex %>" runat="server"/>
                                            </ItemTemplate>
                                    </asp:TemplateField>
                                     <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="Nro.OP">
                                            <ItemTemplate>
                                                <asp:Label ReadOnly="true" ID="nro_op_" runat="server" Text='<%# Eval("nro_op") %>'  Width="50px"></asp:Label>
                                            </ItemTemplate>
                                     </asp:TemplateField>
                                     <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="Siniestro">
                                        <ItemTemplate>
                                            <asp:Label ReadOnly="true" runat="server" Text='<%# Eval("NumeroSiniestro") %>'  Width="80px"></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                     <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="Beneficiario">
                                        <ItemTemplate>
                                            <asp:Label ReadOnly="True" runat="server" Text='<%# Eval("txt_cheque_a_nom") %>' Width="240px"></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                     <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="Monto">
                                        <ItemTemplate>
                                            <asp:Label ReadOnly="true" runat="server" Text='<%# String.Format("{0:#,#0.00}", CDbl(Eval("imp_total")))  %>' CssClass="text-left" Width="80px"></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>       
                                     <asp:TemplateField HeaderStyle-HorizontalAlign="Center" HeaderText="Moneda">
                                        <ItemTemplate>
                                            <asp:Label ReadOnly="true" runat="server" Text='<%# Eval("Moneda") %>'  Width="25px"></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>  

                                      <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="Solicitante">
                                        <ItemTemplate>
                                            <asp:Label ReadOnly="true" ID="Solicitante_" runat="server" Text='<%# Eval("NombreSolicitante") %>' ForeColor='<%# If(CBool(Eval("usu_solrechazo") = Eval("Solicitante")), System.Drawing.Color.White, System.Drawing.Color.Black) %>' BackColor='<%# If(CBool(Eval("Rechazada")), System.Drawing.Color.LightBlue, If(CBool(Eval("usu_solrechazo") = Eval("Solicitante")), System.Drawing.Color.Red, If(CBool(Eval("FirmadoSolicitante")), System.Drawing.Color.LimeGreen, System.Drawing.Color.Orange))) %>'  Width="100px"></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>  
                                     <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="Supervisor">
                                        <ItemTemplate>
                                            <asp:Label ReadOnly="true" ID="Jefe_" runat="server" Text='<%# If(CBool(Eval("Jefe") = "RRAMOS"), "", Eval("NombreJefe"))  %>' ForeColor='<%# If(CBool(Eval("usu_solrechazo") = Eval("Jefe")), System.Drawing.Color.White, System.Drawing.Color.Black) %>' BackColor='<%# If(CBool(Eval("Rechazada")), System.Drawing.Color.LightBlue, If(CBool(Eval("usu_solrechazo") = Eval("Jefe")), System.Drawing.Color.Red, If(CBool(Eval("FirmadoJefe")), System.Drawing.Color.LimeGreen, System.Drawing.Color.Orange))) %>'  Width="100px"  Visible='<%# Eval("NivelAutorizacion") >= 1  %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>  
                                    <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="SubGerente">
                                        <ItemTemplate>
                                            <asp:Label ReadOnly="true" ID="Subgerente_" runat="server" Text='<%# Eval("NombreSubgerente") %>' ForeColor='<%# If(CBool(Eval("usu_solrechazo") = Eval("Subgerente")), System.Drawing.Color.White, System.Drawing.Color.Black) %>' BackColor='<%# If(CBool(Eval("Rechazada")), System.Drawing.Color.LightBlue, If(CBool(Eval("usu_solrechazo") = Eval("Subgerente")), System.Drawing.Color.Red, If(CBool(Eval("FirmadoSubgerente")), System.Drawing.Color.LimeGreen, System.Drawing.Color.Orange))) %>'  Width="100px" visible='<%# Eval("NivelAutorizacion") >= 2  %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>   
                                     <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="SubDirector">
                                        <ItemTemplate>
                                            <asp:Label ReadOnly="true" ID="Subdirector_" runat="server" Text='<%# Eval("NombreSubdirector") %>' ForeColor='<%# If(CBool(Eval("usu_solrechazo") = Eval("Subdirector")), System.Drawing.Color.White, System.Drawing.Color.Black) %>' BackColor='<%# If(CBool(Eval("Rechazada")), System.Drawing.Color.LightBlue, If(CBool(Eval("usu_solrechazo") = Eval("Subdirector")), System.Drawing.Color.Red, If(CBool(Eval("FirmadoSubdirector")), System.Drawing.Color.LimeGreen, System.Drawing.Color.Orange))) %>'  Width="100px" visible='<%# Eval("NivelAutorizacion") >= 3  %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField> 
 
                                     <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="Director">
                                        <ItemTemplate>
                                            <asp:Label ReadOnly="true" ID="Director_" runat="server" Text='<%# Eval("NombreDirector") %>' ForeColor='<%# If(CBool(Eval("usu_solrechazo") = Eval("Director")), System.Drawing.Color.White, System.Drawing.Color.Black) %>' BackColor='<%# If(CBool(Eval("Rechazada")), System.Drawing.Color.LightBlue, If(CBool(Eval("usu_solrechazo") = Eval("Director")), System.Drawing.Color.Red, If(CBool(Eval("FirmadoDirector")), System.Drawing.Color.LimeGreen, System.Drawing.Color.Orange))) %>'  Width="100px" visible='<%# Eval("NivelAutorizacion") >= 4  %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>     
                                     <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="DirectorGeneral">
                                        <ItemTemplate>
                                            <asp:Label ReadOnly="true" ID="DirectorGeneral_" runat="server" Text='<%# Eval("NombreDirectorGeneral") %>' ForeColor='<%# If(CBool(Eval("usu_solrechazo") = Eval("DirectorGeneral")), System.Drawing.Color.White, System.Drawing.Color.Black) %>' BackColor='<%# If(CBool(Eval("Rechazada")), System.Drawing.Color.LightBlue, If(CBool(Eval("usu_solrechazo") = Eval("DirectorGeneral")), System.Drawing.Color.Red, If(CBool(Eval("FirmadoDirectorGeneral")), System.Drawing.Color.LimeGreen, System.Drawing.Color.Orange))) %>' Width="100px"  visible='<%# Eval("NivelAutorizacion") = 5  %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>  

                                    <asp:TemplateField HeaderStyle-HorizontalAlign="Center" HeaderText="Firmar">
                                     <ItemTemplate>
                                            <asp:RadioButton runat="server" TextAlign="Right"  ID="chkFirmar" GroupName="chkAcciones"  Checked='false'/>
                                     </ItemTemplate> 
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderStyle-HorizontalAlign="Center" HeaderText="Rechazar">
                                     <ItemTemplate>
                                            <asp:RadioButton runat="server" TextAlign="Right"  ID="chkSolRechazo" GroupName="chkAcciones"  Checked='false'/>
                                     </ItemTemplate> 
                                    </asp:TemplateField>

                                   <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="MotivoRechazo">
                                    <ItemTemplate>
                                        <asp:DropDownList ID="cmbConcepto" runat="server">
                                              <asp:ListItem Value="0" Text="--Seleccione--"></asp:ListItem>
                                            <asp:ListItem Value="1" Text="Importe incorrecto"></asp:ListItem>
                                            <asp:ListItem Value="2" Text="Error en la cuenta bancaria"></asp:ListItem>
                                            <asp:ListItem Value="3" Text="Error en el concepto de pago"></asp:ListItem> 
                                            <asp:ListItem Value="4" Text="Error en la forma de pago"></asp:ListItem>
                                            <asp:ListItem Value="5" Text="Error en la moneda de pago"></asp:ListItem>
                                            <asp:ListItem Value="6" Text="Beneficiario incorrecto"></asp:ListItem>
                                            <asp:ListItem Value="7" Text="Error en el tipo de pago"></asp:ListItem>
                                            <asp:ListItem Value="8" Text="Error en el número del Siniestro o Subsiniestro"></asp:ListItem>
                                            <asp:ListItem Value="9" Text="Siniestro improcedente de pago"></asp:ListItem>
                                            <asp:ListItem Value="10" Text="No autorizada en tiempo (vencida)"></asp:ListItem>
                                            <%--<asp:ListItem Value="11" Text="Otros (especificar)"></asp:ListItem>--%>
                                        </asp:DropDownList>
                                    </ItemTemplate>
                                    </asp:TemplateField>

                                      <asp:TemplateField HeaderStyle-HorizontalAlign="Center" HeaderText="NoProcede">
                                     <ItemTemplate>
                                            <asp:CheckBox runat="server" TextAlign="Right"  ID="chkNoProc" Checked='false' Visible='<%# If(CBool(Eval("Rechazada")), False, True) %>'/>
                                     </ItemTemplate> 
                                    </asp:TemplateField>
                                      <asp:TemplateField HeaderStyle-HorizontalAlign="Center" HeaderText="2daRev">
                                     <ItemTemplate>
                                            <asp:RadioButton runat="server" TextAlign="Right"  ID="chkSegRev" Checked='false' GroupName="chkAcciones"  Visible='<%#If(Eval("Solicitante") = Master.cod_usuario, False, True) %>'/>
                                     </ItemTemplate> 
                                    </asp:TemplateField>

                                     <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="Motiv.NoProc">
                                        <ItemTemplate>
                                            <asp:TextBox runat="server" ID="txtMotivoNoProc" Width="100px" Visible='<%# If(CBool(Eval("Rechazada")), False, True) %>'></asp:TextBox>
                                        </ItemTemplate>
                                    </asp:TemplateField>  
                                      <asp:TemplateField HeaderStyle-HorizontalAlign="Left" HeaderText="Folio" Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ReadOnly="true" runat="server" ID="folioonbase" Text='<%# Eval("FolioOnbase") %>'  Width="40px" Visible="false" ></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>  
                                 </Columns>
                            </asp:GridView>
                        </asp:Panel>


                  </ContentTemplate>
                    <triggers>
                            <asp:PostBackTrigger ControlID="grdOrdenPago"></asp:PostBackTrigger>
                    </triggers>
                
                </asp:UpdatePanel>
             </div>
            <%--GRID--%>

        <asp:UpdatePanel runat="server" ID="upFirma">
            <ContentTemplate>
                <div class="row" style="width:100%; border-top-style:inset; border-width:1px; border-color:#003A5D">
                    <div class="col-md-1">
                        <div style="width:100%; text-align:right;">
                         
                            <asp:LinkButton id="btn_Firmar" runat="server" class="btn botones" Visible="false">
                                <span>
                                    <img class="btn-aceptar"/>
                                    Aceptar
                                </span>
                            </asp:LinkButton>
                        </div>
                    </div>
                     <div class="col-md-1">
                         <div style="width:100%; text-align:right;">
                            <asp:LinkButton id="btn_Excel" runat="server" class="btn botones" Visible="false">
                                    <span>
                                        <img class="btn-excel"/>
                                        Exportar
                                    </span>
                                </asp:LinkButton>
                        </div> 
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>

     
    </div>

    <%-- <div id="Resumen" style="width:30%" class="modal-catalogo" >
        <asp:UpdatePanel runat="server" ID="upRechazo">
            <ContentTemplate>
                <div class="cuadro-titulo" style="height:30px">
                    <button type="button" class="close"  data-dismiss="modal">&times;</button>
                    <div class="titulo-modal">Resumen de Autorizaciones y Rechazos de Ops</div>
                </div>

                <div class="panel-contenido">  
                    <asp:Label runat="server" Text="PARA AUTORIZAR" Font-Bold="true"></asp:Label>
                    <asp:Panel runat="server" ID="Panel2" Width="100%" Height="200px" ScrollBars="Vertical">
                        <asp:GridView runat="server"  ID="gvd_Autorizadas" AutoGenerateColumns="false"   DataKeyNames=""
                                        GridLines="None"  ShowHeaderWhenEmpty="true" CssClass="grid-view"
                                        HeaderStyle-CssClass="header" >
                            <Columns>
                                <asp:TemplateField HeaderText="No.Orden" HeaderStyle-CssClass="Centro">
                                    <ItemTemplate>
                                        <asp:textbox runat="server" ID="txtOP" CssClass="estandar-control Tablero" Enabled="false" Text='<%# Eval("noOP") %>' Width="100px"></asp:textbox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </asp:Panel>
                    <asp:Label runat="server" Text="PARA CANCELAR" Font-Bold="true"></asp:Label>
                    <asp:Panel runat="server" ID="Panel3" Width="100%" Height="200px" ScrollBars="Vertical">
                        <asp:GridView runat="server"  ID="gvd_Canceladas" AutoGenerateColumns="false"   DataKeyNames=""
                                        GridLines="None"  ShowHeaderWhenEmpty="true" CssClass="grid-view"
                                        HeaderStyle-CssClass="header" >
                            <Columns>
                                <asp:TemplateField HeaderText="No.Orden" HeaderStyle-CssClass="Centro">
                                    <ItemTemplate>
                                        <asp:textbox runat="server" ID="txtNoOP" CssClass="estandar-control Tablero" Enabled="false" Text='<%# Eval("noOP") %>' Width="100px"></asp:textbox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Justificación" HeaderStyle-CssClass="Izquierda">
                                    <ItemTemplate>
                                        <asp:textbox runat="server" ID="txtJustif" CssClass="estandar-control Tablero" Enabled="false" Text='<%# Eval("Justificacion") %>' Width="200px"></asp:textbox>
                                    </ItemTemplate>
                                 </asp:TemplateField>
                            </Columns>
                               
                        </asp:GridView>
                    </asp:Panel>
                </div>
                    <asp:HiddenField runat="server" ID="hid_Token" Value="0" />
                    <asp:Label runat="server" id="lblToken" CssClass="estandar-control Tablero Izquierda" Text="Capture número de token para autorizar" Font-Bold="true"></asp:Label>
                     <asp:textbox runat="server" ID="txtToken" Width="100px" Height="20px"></asp:textbox>
                 <div style="width:100%;text-align:right; border-top-style:inset; border-width:1px; border-color:#003A5D">
                 
                    <asp:LinkButton id="lnkAceptarProc" runat="server" class="btn botones">
                        <span>
                            <img class="btn-aceptar"/>
                            Aceptar
                        </span>
                    </asp:LinkButton>
                    <asp:LinkButton id="lnkCancelaProc" runat="server" data-dismiss="modal" class="btn botones CierraFirma">
                        <span>
                            <img class="btn-cancelar"/>
                            Cancelar
                        </span>
                    </asp:LinkButton>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>--%>


     <div id="Resumen" style="width:30%" class="modal-catalogo" >
        <asp:UpdatePanel runat="server" ID="upRechazo">
            <ContentTemplate>
                <div class="cuadro-titulo" style="height:30px">
                    <button type="button" class="close"  data-dismiss="modal">&times;</button>
                    <div class="titulo-modal">Resumen de Autorizaciones y Rechazos de Ops</div>
                </div>

                <div class="panel-contenido">  
                    <asp:Label ID="TitAutoriza" runat="server" Text="PARA AUTORIZAR" Font-Bold="true"></asp:Label>
                    <asp:Panel runat="server" ID="pnlAutoriza" Width="100%" Height="100px" ScrollBars="Vertical">
                        <asp:GridView runat="server"  ID="gvd_Autorizadas" AutoGenerateColumns="false"   DataKeyNames=""
                                        GridLines="None"  ShowHeaderWhenEmpty="true" CssClass="grid-view"
                                        HeaderStyle-CssClass="header" >
                            <Columns>
                                <asp:TemplateField HeaderText="No.Orden" HeaderStyle-CssClass="Centro">
                                    <ItemTemplate>
                                        <asp:textbox runat="server" ID="txtOP" CssClass="estandar-control Tablero" Enabled="false" Text='<%# Eval("noOP") %>' Width="100px"></asp:textbox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </asp:Panel>
                    <asp:Label id="TitCancel" runat="server" Text="PARA CANCELAR" Font-Bold="true"></asp:Label>
                    <asp:Panel runat="server" ID="pnlCancelar" Width="100%" Height="100px" ScrollBars="Vertical">
                        <asp:GridView runat="server"  ID="gvd_Canceladas" AutoGenerateColumns="false"   DataKeyNames=""
                                        GridLines="None"  ShowHeaderWhenEmpty="true" CssClass="grid-view"
                                        HeaderStyle-CssClass="header" >
                            <Columns>
                                <asp:TemplateField HeaderText="No.Orden" HeaderStyle-CssClass="Centro">
                                    <ItemTemplate>
                                        <asp:textbox runat="server" ID="txtNoOP" CssClass="estandar-control Tablero" Enabled="false" Text='<%# Eval("noOP") %>' Width="100px"></asp:textbox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Justificación" HeaderStyle-CssClass="Izquierda">
                                    <ItemTemplate>
                                        <asp:textbox runat="server" ID="txtJustif" CssClass="estandar-control Tablero" Enabled="false" Text='<%# Eval("Justificacion") %>' Width="200px"></asp:textbox>
                                    </ItemTemplate>
                                 </asp:TemplateField>
                            </Columns>
                               
                        </asp:GridView>
                    </asp:Panel>
                     <asp:Label id="TituloSegRev" runat="server" Text="PARA 2da REVISIÓN" Font-Bold="true"></asp:Label>
                    <asp:Panel runat="server" ID="pnlSegRev" Width="100%" Height="50px" ScrollBars="Vertical">
                        <asp:GridView runat="server"  ID="gvd_segrev" AutoGenerateColumns="false"   DataKeyNames=""
                                        GridLines="None"  ShowHeaderWhenEmpty="true" CssClass="grid-view"
                                        HeaderStyle-CssClass="header" >
                            <Columns>
                                <asp:TemplateField HeaderText="No.Orden" HeaderStyle-CssClass="Centro">
                                    <ItemTemplate>
                                        <asp:textbox runat="server" ID="txtOP_Segrev" CssClass="estandar-control Tablero" Enabled="false" Text='<%# Eval("noOP") %>' Width="100px"></asp:textbox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </asp:Panel>
                    <asp:Label ID="TitNoProc" runat="server" Text="PARA NO PROCEDENTES" Font-Bold="true"></asp:Label>
                    <asp:Panel runat="server" ID="pnlNoProc" Width="100%" Height="50px" ScrollBars="Vertical">
                        <asp:GridView runat="server"  ID="gvd_noproc" AutoGenerateColumns="false"   DataKeyNames=""
                                        GridLines="None"  ShowHeaderWhenEmpty="true" CssClass="grid-view"
                                        HeaderStyle-CssClass="header" >
                            <Columns>
                                <asp:TemplateField HeaderText="No.Orden" HeaderStyle-CssClass="Centro">
                                    <ItemTemplate>
                                        <asp:textbox runat="server" ID="txtNoOP_Noproc" CssClass="estandar-control Tablero" Enabled="false" Text='<%# Eval("noOP") %>' Width="100px"></asp:textbox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Justificación" HeaderStyle-CssClass="Izquierda">
                                    <ItemTemplate>
                                        <asp:textbox runat="server" ID="txtJustif_NoProc" CssClass="estandar-control Tablero" Enabled="false" Text='<%# Eval("Justificacion") %>' Width="200px"></asp:textbox>
                                    </ItemTemplate>
                                 </asp:TemplateField>
                            </Columns>
                               
                        </asp:GridView>
                    </asp:Panel>
                </div>
                    <asp:HiddenField runat="server" ID="hid_Token" Value="0" />
                    <asp:Label runat="server" id="lblToken" CssClass="estandar-control Tablero Izquierda" Text="Capture número de token para autorizar" Font-Bold="true"></asp:Label>
                     <asp:textbox runat="server" ID="txtToken" Width="100px" Height="20px"></asp:textbox>
                 <div style="width:100%;text-align:right; border-top-style:inset; border-width:1px; border-color:#003A5D">
                 
                    <asp:LinkButton id="lnkAceptarProc" runat="server" class="btn botones">
                        <span>
                            <img class="btn-aceptar"/>
                            Aceptar
                        </span>
                    </asp:LinkButton>
                    <asp:LinkButton id="lnkCancelaProc" runat="server" data-dismiss="modal" class="btn botones CierraFirma">
                        <span>
                            <img class="btn-cancelar"/>
                            Cancelar
                        </span>
                    </asp:LinkButton>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>


 </asp:Content>