<%@ Page Title="" Language="VB" MasterPageFile="~/Pages/SiteMaster.master" AutoEventWireup="false" CodeFile="ABM_AnalistaSolicitante.aspx.vb" Inherits="Siniestros_ABM_AnalistaSolicitante" %>

<%@ MasterType VirtualPath="~/Pages/SiteMaster.master" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cph_principal" runat="Server">
    <script src="../Scripts/Siniestros/DatosEnvioCheque.js"></script>
    <asp:HiddenField runat="server" ID="hid_Ventanas" Value="0|1" />
    <div class="zona-principal" style="overflow-x: hidden; overflow-y: hidden">
        <div class="cuadro-titulo panel-encabezado" style="text-align: left; tab-size: 18px">
            <input type="image" src="../Images/contraer_mini_inv.png" id="coVentana0" class="contraer" />
            <input type="image" src="../Images/expander_mini_inv.png" id="exVentana0" class="expandir" />
            &nbsp&nbsp Analista Solicitantes
        </div>
        <div class="padding10"></div>


        <div class="row">


            <asp:UpdatePanel runat="server" ID="upGenerales" UpdateMode="Conditional">
                <ContentTemplate>


                    <div class="form-group-sm col-md-2">
                        <asp:Label runat="server" class="etiqueta-control">Codigo de Usuario</asp:Label>

                        <div class="input-group">
                            <asp:TextBox ID="txtCodigoUsuario" runat="server" CssClass="estandar-control Tablero" placeholder="Codigo Usuario"></asp:TextBox>
                            <div class="input-group-btn">
                                <asp:LinkButton ID="btnConsulta" runat="server" class="btn btn-primary btn-xs pull-left Tablero" Style="background-color: #003A5D;">...</asp:LinkButton>
                            </div>
                        </div>

                    </div>


                    <div class="form-group col-md-3">
                        <asp:Label runat="server" class="etiqueta-control">Nombre del Analista</asp:Label>
                        <asp:TextBox AutoPostBack="True" ID="txtNombre" runat="server" CssClass="estandar-control siniestro Tablero" placeholder="Nombre Analista" disabled></asp:TextBox>

                    </div>


                    <div class="form-group col-md-4">
                        <asp:Label runat="server" class="etiqueta-control">Origen de Pago</asp:Label>
                        <asp:DropDownList ID="cmbOrigen" runat="server" ClientIDMode="Static" CssClass="estandar-control Tablero">
                        </asp:DropDownList>
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="btnAgregar" EventName="Click" />
                </Triggers>
            </asp:UpdatePanel>
            <div class="form-group col-md-3">
                <br />
                <asp:LinkButton ID="btnAgregar" runat="server" class="btn btn-primary btn-xs pull-left Tablero" Style="background-color: #003A5D;">Agregar</asp:LinkButton>
            </div>





        </div>
        <div style="width: 100%; text-align: right; border-top-style: inset; border-width: 1px; border-color: #003A5D"></div>
        <br />
        <div class="row">
            <center>
                   <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional" >
                       <ContentTemplate>

                                       <asp:Panel runat="server" ID="pnlUsuario" Width="100%" Height="300" ScrollBars="Auto" >
                                         <asp:GridView ID="grd" runat="server" AutoGenerateColumns="False" BackColor="White" BorderColor="#003A5D" BorderStyle="None" BorderWidth="1px" CellPadding="3" GridLines="Vertical" Width="95%">
                                             <AlternatingRowStyle BackColor="#DCDCDC" />
                                             <Columns>
                                                 
                                                 <asp:BoundField DataField="usuarioanalista" HeaderText="Usuario" />
                                                 <asp:BoundField DataField="AnalistaFondos" HeaderText="Nombre" />
                                                 <asp:BoundField DataField="desc_origen_pago" HeaderText="Origen Pago" />
                                         
                                                  <asp:TemplateField HeaderText="Activo" ItemStyle-HorizontalAlign="Center" ShowHeader="False">
                                                 <ItemTemplate>
                                                        <asp:CheckBox ID="CheckBox1" runat="server" Checked='<%# Eval("sn_activo") %>' />
                                                        <asp:HiddenField ID="cod_origen_pago" runat="server" Value='<%#Eval("cod_origen_pago") %>'/>
                                                  </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:TemplateField>

                                                 <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-Width ="15px">
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
                            <asp:AsyncPostBackTrigger ControlID="btnStatus" EventName="Click" />
                            
                       </Triggers>
                   </asp:UpdatePanel> 
                      </center>
        </div>
        <br />
        <div class="row">
            <div class="form-group col-md-12" style="text-align: right">
                <asp:LinkButton ID="btnStatus" runat="server" class="btn botones" Width="150px" Height="30px" Style="background-color: #003A5D;">Guardar Status</asp:LinkButton>
            </div>
        </div>

        <br />
        <br />
        <br />
    </div>

</asp:Content>

