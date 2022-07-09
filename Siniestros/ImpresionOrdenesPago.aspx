<%@ Page Title="" Language="VB" MasterPageFile="~/Pages/SiteMaster.master" AutoEventWireup="false" CodeFile="ImpresionOrdenesPago.aspx.vb" Inherits="Siniestros_ImpresionOrdenesPago" %>


<%@ MasterType VirtualPath="~/Pages/SiteMaster.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph_principal" runat="Server">
    <script src="../Scripts/Siniestros/PagoInter.js"></script>
    <asp:HiddenField runat="server" ID="HiddenField2" Value="0|1" />
        
    <div class="zona-principal" style="overflow-x:hidden;overflow-y:hidden">
        <div class="cuadro-titulo panel-encabezado" style="text-align: left; tab-size: 18px">
            <input type="image" src="../Images/contraer_mini_inv.png" id="coVentana0" class="contraer" />
            <input type="image" src="../Images/expander_mini_inv.png" id="exVentana0" class="expandir" />
            Filtros - Impresi&oacute;n de Ordenes de pago
        </div>

        <div class="panel-contenido ventana0" >
            <asp:UpdatePanel runat="server" ID="upFiltros">
                <ContentTemplate>
                    <asp:HiddenField runat="server" ID="hid_Ventanas" Value="0|1|1|1|1|1|1|1|" />

                    <div class="row">
                        <div class="col-md-6">
                            <asp:label runat="server" class="col-md-1 etiqueta-control" Width="20%">Orden Pago</asp:label>
                            <asp:TextBox runat="server" ID="txt_NroOP" CssClass="col-md-1 estandar-control" onkeypress="return solonumerosyseparador(event)" PlaceHolder="Ejemplo: 84162" Width="80%"></asp:TextBox>
                        </div>
                        <div class="col-md-6">
                            <asp:label runat="server" class="col-md-1 etiqueta-control" Width="25%">Beneficiario</asp:label>
                            <asp:HiddenField runat="server" ID="hidClaveAse" Value="" />
                            <asp:textbox runat="server" ID="txt_beneficiario" CssClass="estandar-control" Width="75%" ></asp:textbox>
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
                            <asp:label runat="server" class="col-md-1 etiqueta-control" Width="25%">Elaborado por</asp:label>
                            <asp:DropDownList runat="server" ID="cmbElaborado" CssClass="estandar-control" Width="75%" ></asp:DropDownList>
                        </div>

                    </div>

                     <div class="clear padding5"></div>

                     <div class="row">
                        <div class="col-md-6">
                             <asp:label runat="server" class="col-md-1 etiqueta-control" Width="20%">Siniestro</asp:label>
                            <asp:textbox runat="server" ID="txtSiniestro" CssClass="estandar-control" Width="80%" onkeypress="return soloNumeros(event)"></asp:textbox>
                        </div>
                         <div class="col-md-6">
                            <asp:label runat="server" class="col-md-1 etiqueta-control" Width="25%">Folio Onbase</asp:label>
                            <asp:textbox runat="server" ID="TextFolio" CssClass="estandar-control" Width="75%" onkeypress="return soloNumeros(event)" PlaceHolder="Folio Onbase"></asp:textbox>
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
                                        <td><asp:label runat="server" class="etiqueta-control">Estatus Ordenes de Pago:</asp:label></td>
                                        <td><asp:RadioButton runat="server" ID="chk_Todas" Text="Todas" CssClass="etiqueta-control" Width="150px" GroupName="FiltrosG" Visible="true" /></td>
                                        <td><asp:RadioButton runat="server" ID="chk_Rechazadas" Text="Canceladas" CssClass="etiqueta-control"  Width="150px" GroupName="FiltrosG" Visible="true"  /></td>
                                        <td><asp:RadioButton runat="server" ID="chk_MisVig" Text="Vigentes" CssClass="etiqueta-control" Width="150px"  GroupName="FiltrosG" Visible="true"  /></td>
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
                    <asp:LinkButton id="btn_BuscarOP" runat="server" class="btn botones">
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
             <div class="clear padding5"></div>

       <div class="padding20"></div>

    <div class="row" >
        <center>
            <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional" >
                <ContentTemplate>
                    <asp:Panel runat="server" ID="pnlUsuario" Width="100%" Height="300" ScrollBars="Auto" >
                    <%-- OnRowCommand="grd_RowCommand" <asp:GridView ID="grd" runat="server" AutoGenerateColumns="False" BackColor="White" BorderColor="#003A5D" BorderStyle="None" BorderWidth="1px" CellPadding="3" GridLines="Vertical" Width="95%">--%>                                           
                        <asp:GridView ID="grd" runat="server" AutoGenerateColumns="False"  DataKeyNames="nro_OP" CssClass="table-condensed table-hover" Font-Size="11px" GridLines="Vertical" HeaderStyle-CssClass="header" Height="35px" HorizontalAlign="Center" ShowHeaderWhenEmpty="True">
                            <AlternatingRowStyle BackColor="#DCDCDC" />
                            <Columns>
                                <asp:TemplateField HeaderText="Imprimir" ItemStyle-HorizontalAlign="Center" ShowHeader="False">
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chk_Print" runat="server" Checked="False" />
                                    </ItemTemplate>
                                    <HeaderStyle Font-Size="12px" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:BoundField DataField="Estatus" HeaderText="Estatus" ItemStyle-Width="200px" />
                                <asp:BoundField DataField="Fecha" HeaderText="Fecha Generacion" ItemStyle-Width="110px" DataFormatString="{0:dd/MM/yyyy}"/>                                 
                                <asp:BoundField DataField="nro_OP" HeaderText="Nro.OP" ItemStyle-Width="80px" />
                                <asp:BoundField DataField="Siniestro" HeaderText="Siniestro" ItemStyle-Width="80px" />
                                <asp:BoundField DataField="Beneficiario" HeaderText="Beneficiario" ItemStyle-Width="500px" />
                                <asp:BoundField DataField="Moneda" HeaderText="Moneda" ItemStyle-Width="130px" />
                                <asp:BoundField ItemStyle-CssClass="t-cost" DataField="Total" HeaderText="Monto" ItemStyle-Width="130px" DataFormatString="{0:c}" ItemStyle-HorizontalAlign="right" />

                            </Columns>
                            <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
                            <HeaderStyle BackColor="#003A5D" Font-Bold="False" ForeColor="White" />
                            <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                            <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
                            <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
                            <SortedAscendingCellStyle BackColor="#F1F1F1" />
                            <SortedAscendingHeaderStyle BackColor="#0000A9" />
                            <SortedDescendingCellStyle BackColor="#CAC9C9" />
                            <SortedDescendingHeaderStyle BackColor="#000065" />
                        </asp:GridView>
                    </asp:Panel>
                </ContentTemplate>
                <Triggers>                           
                    <asp:AsyncPostBackTrigger ControlID="btn_BuscarOP" EventName="Click" />                            
                    <asp:AsyncPostBackTrigger ControlID="btn_Todas" EventName="Click" />                            
                    <asp:AsyncPostBackTrigger ControlID="btn_Ninguna" EventName="Click" />                            
                    <asp:AsyncPostBackTrigger ControlID="btn_Imprimir" EventName="Click" />                            
                </Triggers>
            </asp:UpdatePanel> 
        </center>
    </div>

       <div class="padding20">       
            <asp:UpdatePanel runat="server" ID="upPanelBotones" Width="99%" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:Panel runat="server" ID="PanelBotones" HorizontalAlign="Right">
                    <div class="row" style="width: 100%">
                        <%--<div class="col-md-1"></div>--%>
                        <div class="col-md-6">
                            <div style="width: 100%; text-align: left">
                                
                                <asp:LinkButton ID="btn_Todas" runat="server" class="btn botones Centrado" BorderWidth="2" BorderColor="White" Width="150px" Visible="false">
                                    <span>
                                        <img class="btn-todos"/>&nbsp Seleccionar todas
                                    </span>
                                </asp:LinkButton>

                                <asp:LinkButton ID="btn_Ninguna" runat="server" class="btn botones Centrado" BorderWidth="2" BorderColor="White" Width="150px" Visible="false">
                                    <span>
                                        <img class="btn-ninguno"/>&nbsp Limpiar selección
                                    </span>
                                </asp:LinkButton>

                            </div>
                        </div>
                        <div class="col-md-6">                            
                            <div style="text-align: right">  
                                <asp:LinkButton ID="btn_Imprimir" runat="server" class="btn botones" BorderWidth="2" BorderColor="White" Width="150px" Visible="false">
                                <span>
                                    <img class="btn-imprimir"/>
                                    Imprimir
                                </span>
                                </asp:LinkButton>
                                <asp:LinkButton ID="btnEnviar" runat="server" class="btn botones Centrado" BorderWidth="2" BorderColor="White" Width="190px" Visible="false">
                                    <span>
                                        <img class="btn-imprimir"/>&nbsp Imprimir todas en un PDF
									</span>
                                </asp:LinkButton>
                                          
                               <%-- <asp:LinkButton ID="btn_Continuar" runat="server" class="btn botones" Width="153px" data-toggle="modal" data-target="#mailPI" Visible="true">
                                <span>
                                    <img class="btn-aceptar"/>&nbspContinuar
                                </span>
                            </asp:LinkButton>--%>
                            </div>
                        </div>                        
                    </div>
                    <div class="padding20" />
                </asp:Panel>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="btn_BuscarOP" EventName="Click" />
                <%--<asp:AsyncPostBackTrigger ControlID="btnSi" EventName="Click" />--%>
            </Triggers>
        </asp:UpdatePanel>
   <%-- </div>--%>
    </div>

    <div class="padding53"></div>

 </asp:Content>

    




