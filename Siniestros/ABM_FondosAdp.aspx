<%@ Page Title="" Language="VB" MasterPageFile="~/Pages/SiteMaster.master" AutoEventWireup="false" CodeFile="ABM_FondosAdp.aspx.vb" Inherits="Siniestros_ABM_FondosAdp" %>

<asp:Content ID="Content2" ContentPlaceHolderID="cph_principal" runat="Server">
    <script src="../Scripts/Siniestros/DatosEnvioCheque.js"></script>
    <asp:HiddenField runat="server" ID="hid_Ventanas" Value="0|1" />
    <div class="zona-principal" style="overflow-x: hidden; overflow-y: hidden">
        <div class="cuadro-titulo panel-encabezado" style="text-align: left; tab-size: 18px">
            <input type="image" src="../Images/contraer_mini_inv.png" id="coVentana0" class="contraer" />
            <input type="image" src="../Images/expander_mini_inv.png" id="exVentana0" class="expandir" />
            &nbsp&nbsp Concepto de Fondos ADP
        </div>
        <div class="padding30"></div>


        <div class="row">


            <asp:UpdatePanel runat="server" ID="upGenerales" UpdateMode="Conditional">
                <ContentTemplate>

             

                    <div class="form-group col-md-2">
                        <asp:Label runat="server" class="etiqueta-control">Clase de Pago</asp:Label>
                        <asp:DropDownList ID="ddl_clase_pago" runat="server" ClientIDMode="Static" CssClass="estandar-control Tablero" AutoPostBack="true">
                        </asp:DropDownList>
                    </div>


                    <div class="form-group col-md-4">
                        <asp:Label runat="server" class="etiqueta-control">Concepto de Pago</asp:Label>
                        <asp:DropDownList ID="ddl_concepto_pago" runat="server" ClientIDMode="Static" CssClass="estandar-control Tablero">
                        </asp:DropDownList>

                    </div>


                    <div class="form-group col-md-2">
                        <asp:Label runat="server" class="etiqueta-control">Origen de Pago</asp:Label>
                        <asp:DropDownList ID="cmbOrigen" runat="server" ClientIDMode="Static" CssClass="estandar-control Tablero">
                        </asp:DropDownList>
                    </div>

                    <div class="form-group col-md-3">
                        <asp:Label runat="server" class="etiqueta-control">Descripcion del Concepto</asp:Label>
                        <asp:TextBox AutoPostBack="false" ID="txtNombre" runat="server" CssClass="estandar-control siniestro Tablero" placeholder="Concepto"></asp:TextBox>
                    </div>


                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="btnAgregar" EventName="Click" />
                    <asp:AsyncPostBackTrigger ControlID="ddl_clase_pago" EventName="SelectedIndexChanged" />
                </Triggers>
            </asp:UpdatePanel>
            <div class="form-group col-md-1">
                <br />
                <asp:LinkButton ID="btnAgregar" runat="server" class="btn btn-primary btn-xs pull-left Tablero" Style="background-color: #003A5D;">Agregar</asp:LinkButton>
            </div>




        </div>
        <div style="width: 100%; text-align: right; border-top-style: inset; border-width: 1px; border-color: #003A5D"></div>
        <br />
        <div class="row">
            <div class="col-md-12" style="text-align: right">
                <asp:LinkButton ID="btnExportar" runat="server" class="btn botones" Width="150px" Height="30px">
                                <span>
                                    <img class="btn-excel"/>&nbsp&nbsp Exportar Catálogo
                                </span>
                </asp:LinkButton>
            </div>
        </div>
        <br />
        <div class="row">
            <center>
                   <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional" >
                       <ContentTemplate>
                            <asp:Panel runat="server" ID="pnlUsuario" Width="100%" Height="300" ScrollBars="Auto" >
                                <asp:GridView ID="grd" runat="server" AutoGenerateColumns="False" BackColor="White" BorderColor="#003A5D" BorderStyle="None" BorderWidth="1px" CellPadding="3" GridLines="Vertical" Width="95%">
                                    <AlternatingRowStyle BackColor="#DCDCDC" />
                                    <Columns>                                                 
                                        <asp:BoundField DataField="Concepto" HeaderText="Concepto" />
                                        <asp:BoundField DataField="Descripcion" HeaderText="Descripcion" />
                                        <asp:BoundField DataField="cod_cta_cble" HeaderText="Cuenta Contable" />
                                        <asp:BoundField DataField="cod_origen_pago" HeaderText="Origen Pago" />
                                        <asp:BoundField DataField="desc_origen_pago" HeaderText="Descripción" />
                                        <asp:BoundField DataField="cod_clase_pago" HeaderText="Clase de Pago" />
                                        <asp:BoundField DataField="clase_pago" HeaderText="Descripcion" />                                        
                                        <asp:BoundField DataField="concepto_detalle" HeaderText="Concepto del detalle" />
                                        
                                        
                                        <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:ImageButton ImageUrl="~/Images/delete_rojo.png" CommandName="Delete" runat="server" CssClass="btn Delete" Height="25" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
             
                                    </Columns>
                                    <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
                                    <HeaderStyle BackColor="#003A5D" Font-Bold="True" ForeColor="White" />
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
                           <asp:AsyncPostBackTrigger ControlID="btnAgregar" EventName="Click" />
                       </Triggers>
                   </asp:UpdatePanel> 
                      </center>
        </div>



    </div>

</asp:Content>



