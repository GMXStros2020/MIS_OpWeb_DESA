<%@ Page Title="" Language="VB" MasterPageFile="~/Pages/SiteMaster.master" AutoEventWireup="false" CodeFile="ABM_PaisesParaPago.aspx.vb" Inherits="Siniestros_ABM_AnalistaSolicitante" %>
<%@ MasterType VirtualPath="~/Pages/SiteMaster.master" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cph_principal" runat="Server">
      <script src="../Scripts/Siniestros/DatosEnvioCheque.js"></script>
     <asp:HiddenField runat="server" ID="hid_Ventanas" Value="0|1" />
        <div class="zona-principal" style="overflow-x: hidden; overflow-y: hidden">
            <div class="cuadro-titulo panel-encabezado" style="text-align: left; tab-size: 18px">
                           <input type="image" src="../Images/contraer_mini_inv.png" id="coVentana0" class="contraer" />
            <input type="image" src="../Images/expander_mini_inv.png" id="exVentana0" class="expandir"/>
                   &nbsp&nbsp Paises para pagos Internacionales
            </div>
             <div class="padding10"></div>


              
                 
                <asp:UpdatePanel runat="server" ID="upGenerales" UpdateMode="Conditional" >
                           <ContentTemplate>
                                         
                            <div class="row">                             
                                     <div class="form-group col-md-4">
                                        <asp:Label runat="server" class="etiqueta-control">Pais</asp:Label>
                                        <asp:DropDownList ID="cmbPais" runat="server" ClientIDMode="Static" CssClass="estandar-control Tablero">
                                        </asp:DropDownList>
                                    </div>



                               
                              </div>
                               <div class="row">
                                   <div class="form-group col-md-3">
                                            <div class="form-check">
                                                <asp:CheckBox runat="server" ID="chkBanco" Text=" Banco" CssClass="etiqueta-control" />
                                            </div>
                                    </div>
                                    <div class="form-group col-md-3">
                                            <div class="form-check">
                                                <asp:CheckBox runat="server" ID="chkNumBanco" Text=" Num de banco" CssClass="etiqueta-control" />
                                            </div>
                                    </div>
                                    <div class="form-group col-md-3">
                                            <div class="form-check ">
                                                <asp:CheckBox runat="server" ID="chkDomicilio" Text=" Domicilio Banco" CssClass="etiqueta-control" />
                                            </div>
                                    </div>
                                    <div class="form-group col-md-3">
                                            <div class="form-check ">
                                                <asp:CheckBox runat="server" ID="chkCuenta" Text=" Cuenta" CssClass="etiqueta-control" />
                                            </div>
                                    </div>
                               </div>
                                <div class="row">
                                            <div class="form-group col-md-3">
                                            <div class="form-check ">
                                                <asp:CheckBox runat="server" ID="chkAba" Text=" Aba o Routing" CssClass="etiqueta-control" />
                                            </div>
                                    </div>
                                    <div class="form-group col-md-3">
                                            <div class="form-check ">
                                                <asp:CheckBox runat="server" ID="chkswift" Text=" Swift" CssClass="etiqueta-control" />
                                            </div>
                                    </div>
                                    <div class="form-group col-md-3">
                                            <div class="form-check ">
                                                <asp:CheckBox runat="server" ID="chkTransit" Text=" Transit" CssClass="etiqueta-control" />
                                            </div>
                                    </div>
                                    <div class="form-group col-md-3">
                                            <div class="form-check ">
                                                <asp:CheckBox runat="server" ID="chkIban" Text=" Iban" CssClass="etiqueta-control" />
                                            </div>
                                    </div>
                                </div>
                               <div class="row">
                                   <div class="form-group col-md-12">
                                     <br />
                                        <asp:LinkButton ID="btnAgregar" runat="server" class="btn btn-primary btn-xs pull-right Tablero" Style="background-color: #003A5D;">Agregar</asp:LinkButton>
                                   </div>

                               </div>

                  </ContentTemplate>
                    <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="btnAgregar" EventName="Click" />
                   </Triggers>
                </asp:UpdatePanel>  


      
                  
    

                <div style="width:100%; text-align:right; border-top-style:inset; border-width:1px; border-color:#003A5D"></div>
            <br />
              <div class="row">
                  <center>
                   <asp:UpdatePanel runat="server" ID="UpdatePanel1" UpdateMode="Conditional" >
                       <ContentTemplate>

                                       <asp:Panel runat="server" ID="pnlUsuario" Width="100%" Height="300" ScrollBars="Auto" >
                                         <asp:GridView ID="grd" runat="server" AutoGenerateColumns="False" BackColor="White" BorderColor="#003A5D" BorderStyle="None" BorderWidth="1px" CellPadding="3" GridLines="Vertical" Width="95%">
                                             <AlternatingRowStyle BackColor="#DCDCDC" />
                                             <Columns>
                                                 
                                                 <asp:BoundField DataField="txt_desc" HeaderText="Pais" />

                                                  <asp:TemplateField HeaderText="Banco" ItemStyle-HorizontalAlign="Center" ShowHeader="False">
                                                 <ItemTemplate>
                                                        <asp:CheckBox ID="sn_banco" runat="server" Checked='<%# Eval("sn_banco") %>' />
                                                        <asp:HiddenField ID="cod_pais" runat="server" Value='<%#Eval("cod_pais") %>'/>
                                                  </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:TemplateField>
                                                 
                                                 <asp:TemplateField HeaderText="Num de Banco" ItemStyle-HorizontalAlign="Center" ShowHeader="False">
                                                 <ItemTemplate>
                                                        <asp:CheckBox ID="sn_num_banco" runat="server" Checked='<%# Eval("sn_num_banco") %>' />
                                                        
                                                  </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:TemplateField>

                                                 <asp:TemplateField HeaderText="Domicilio del Banco" ItemStyle-HorizontalAlign="Center" ShowHeader="False">
                                                 <ItemTemplate>
                                                        <asp:CheckBox ID="sn_domicilio" runat="server" Checked='<%# Eval("sn_domicilio") %>' />
                                                        
                                                  </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:TemplateField>

                                                 <asp:TemplateField HeaderText="Cuenta" ItemStyle-HorizontalAlign="Center" ShowHeader="False">
                                                 <ItemTemplate>
                                                        <asp:CheckBox ID="sn_cuenta" runat="server" Checked='<%# Eval("sn_cuenta") %>' />
                                                        
                                                  </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:TemplateField>


                                                 <asp:TemplateField HeaderText="Aba o Routing" ItemStyle-HorizontalAlign="Center" ShowHeader="False">
                                                 <ItemTemplate>
                                                        <asp:CheckBox ID="sn_aba_routing" runat="server" Checked='<%# Eval("sn_aba_routing") %>' />
                                                        
                                                  </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:TemplateField>


                                                 <asp:TemplateField HeaderText="Swift" ItemStyle-HorizontalAlign="Center" ShowHeader="False">
                                                 <ItemTemplate>
                                                        <asp:CheckBox ID="sn_swift" runat="server" Checked='<%# Eval("sn_swift") %>' />
                                                        
                                                  </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:TemplateField>


                                                 <asp:TemplateField HeaderText="Transit" ItemStyle-HorizontalAlign="Center" ShowHeader="False">
                                                 <ItemTemplate>
                                                        <asp:CheckBox ID="sn_transit" runat="server" Checked='<%# Eval("sn_transit") %>' />
                                                        
                                                  </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Iban" ItemStyle-HorizontalAlign="Center" ShowHeader="False">
                                                 <ItemTemplate>
                                                        <asp:CheckBox ID="sn_iban" runat="server" Checked='<%# Eval("sn_iban") %>' />
                                                        
                                                  </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:TemplateField>

                                                 <%----FJCP 10290_CC INI--%>
                                                <asp:TemplateField HeaderText="Eliminar" ItemStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <asp:ImageButton ImageUrl="~/Images/delete_rojo.png" CommandName="Delete" runat="server" CssClass="btn Delete" Height="25" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>                                            
                                                 <%----FJCP 10290_CC FIN--%>

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
                <div class="form-group col-md-12">                             
                    <asp:LinkButton ID="btnStatus" runat="server" class="btn btn-primary btn-xs pull-right Tablero" Style="background-color: #003A5D;">Guardar</asp:LinkButton>
                </div>
            </div>  
            <br />
            <br />
            <br />
        </div>
 
</asp:Content>
