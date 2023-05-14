<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        components = New ComponentModel.Container()
        Dim resources As ComponentModel.ComponentResourceManager = New ComponentModel.ComponentResourceManager(GetType(Form1))
        btnGuardar = New Button()
        cboTablas = New ComboBox()
        Label3 = New Label()
        btnMostrarTablas = New Button()
        txtCodigo = New TextBox()
        btnGenerarClase = New Button()
        Label5 = New Label()
        txtClase = New TextBox()
        btnSalir = New Button()
        Label7 = New Label()
        txtSelect = New TextBox()
        grbAccess = New GroupBox()
        Label4 = New Label()
        txtAccessPassword = New TextBox()
        Label6 = New Label()
        txtAccessProvider = New TextBox()
        btnExaminar = New Button()
        txtAccessNombreBase = New TextBox()
        Label8 = New Label()
        grbSQL = New GroupBox()
        chkUsarSQLEXpress = New CheckBox()
        chkSeguridadSQL = New CheckBox()
        txtSqlPassword = New TextBox()
        txtSqlUserId = New TextBox()
        txtSqlInitialCatalog = New TextBox()
        txtSqlDataSource = New TextBox()
        labelPassw = New Label()
        labelUser = New Label()
        Label2 = New Label()
        Label1 = New Label()
        Panel1 = New Panel()
        optCS = New RadioButton()
        optVB = New RadioButton()
        optAccess = New RadioButton()
        optSQL = New RadioButton()
        toolTip1 = New ToolTip(components)
        chkUsarCommandBuilder = New CheckBox()
        chkUsarAddWithValue = New CheckBox()
        chkUsarDataAdapter = New CheckBox()
        chkUsarOverrides = New CheckBox()
        chkPropiedadAuto = New CheckBox()
        chkCrearIndizador = New CheckBox()
        grbOpciones = New GroupBox()
        StatusStrip1 = New StatusStrip()
        LabelInfo = New ToolStripStatusLabel()
        LabelVersion = New ToolStripStatusLabel()
        LabelCodigo = New Label()
        grbAccess.SuspendLayout()
        grbSQL.SuspendLayout()
        Panel1.SuspendLayout()
        grbOpciones.SuspendLayout()
        StatusStrip1.SuspendLayout()
        SuspendLayout()
        ' 
        ' btnGuardar
        ' 
        btnGuardar.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        btnGuardar.FlatStyle = FlatStyle.System
        btnGuardar.Location = New Point(965, 1188)
        btnGuardar.Margin = New Padding(3, 4, 3, 4)
        btnGuardar.Name = "btnGuardar"
        btnGuardar.Size = New Size(165, 42)
        btnGuardar.TabIndex = 13
        btnGuardar.Text = "&Guardar"
        toolTip1.SetToolTip(btnGuardar, " Guardar la clase generada (se guardará con el nombre de la tabla y la extensión del lenguaje) ")
        ' 
        ' cboTablas
        ' 
        cboTablas.DropDownStyle = ComboBoxStyle.DropDownList
        cboTablas.Location = New Point(208, 483)
        cboTablas.Margin = New Padding(3, 4, 3, 4)
        cboTablas.Name = "cboTablas"
        cboTablas.Size = New Size(334, 33)
        cboTablas.TabIndex = 5
        ' 
        ' Label3
        ' 
        Label3.Location = New Point(24, 486)
        Label3.Name = "Label3"
        Label3.Size = New Size(178, 37)
        Label3.TabIndex = 4
        Label3.Text = "Tablas:"
        toolTip1.SetToolTip(Label3, " Lista de las tablas de la base de datos ")
        ' 
        ' btnMostrarTablas
        ' 
        btnMostrarTablas.FlatStyle = FlatStyle.System
        btnMostrarTablas.Location = New Point(563, 477)
        btnMostrarTablas.Margin = New Padding(3, 4, 3, 4)
        btnMostrarTablas.Name = "btnMostrarTablas"
        btnMostrarTablas.Size = New Size(192, 42)
        btnMostrarTablas.TabIndex = 6
        btnMostrarTablas.Text = "Mostrar &tablas"
        toolTip1.SetToolTip(btnMostrarTablas, " Mostrar las tablas que contiene la base de datos indicada ")
        ' 
        ' txtCodigo
        ' 
        txtCodigo.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        txtCodigo.Font = New Font("Courier New", 8.25F, FontStyle.Regular, GraphicsUnit.Point)
        txtCodigo.Location = New Point(18, 797)
        txtCodigo.Margin = New Padding(3, 4, 3, 4)
        txtCodigo.Multiline = True
        txtCodigo.Name = "txtCodigo"
        txtCodigo.ScrollBars = ScrollBars.Both
        txtCodigo.Size = New Size(914, 477)
        txtCodigo.TabIndex = 12
        txtCodigo.Text = "TextBox1"
        txtCodigo.WordWrap = False
        ' 
        ' btnGenerarClase
        ' 
        btnGenerarClase.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        btnGenerarClase.FlatStyle = FlatStyle.System
        btnGenerarClase.Location = New Point(914, 102)
        btnGenerarClase.Margin = New Padding(3, 4, 3, 4)
        btnGenerarClase.Name = "btnGenerarClase"
        btnGenerarClase.Size = New Size(172, 42)
        btnGenerarClase.TabIndex = 7
        btnGenerarClase.Text = "Generar &Clase"
        toolTip1.SetToolTip(btnGenerarClase, " Generar el código de la clase en el lenguaje indicado ")
        ' 
        ' Label5
        ' 
        Label5.Location = New Point(24, 595)
        Label5.Name = "Label5"
        Label5.Size = New Size(178, 37)
        Label5.TabIndex = 9
        Label5.Text = "Clase:"
        toolTip1.SetToolTip(Label5, " Nombre de la clase a generar (por defecto tendrá el mismo nombre que la tabla) ")
        ' 
        ' txtClase
        ' 
        txtClase.Location = New Point(208, 592)
        txtClase.Margin = New Padding(3, 4, 3, 4)
        txtClase.Name = "txtClase"
        txtClase.Size = New Size(291, 31)
        txtClase.TabIndex = 10
        txtClase.Text = "TextBox1"
        ' 
        ' btnSalir
        ' 
        btnSalir.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        btnSalir.FlatStyle = FlatStyle.System
        btnSalir.Location = New Point(965, 1238)
        btnSalir.Margin = New Padding(3, 4, 3, 4)
        btnSalir.Name = "btnSalir"
        btnSalir.Size = New Size(165, 40)
        btnSalir.TabIndex = 14
        btnSalir.Text = "Salir"
        toolTip1.SetToolTip(btnSalir, " Terminar la aplicación ")
        ' 
        ' Label7
        ' 
        Label7.Location = New Point(24, 544)
        Label7.Name = "Label7"
        Label7.Size = New Size(178, 37)
        Label7.TabIndex = 7
        Label7.Text = "Cadena select:"
        toolTip1.SetToolTip(Label7, " Cadena de selección de los elementos a usar por la clase (SELECT * FROM <tabla> WHERE <condición> ORDER BY <campo>) ")
        ' 
        ' txtSelect
        ' 
        txtSelect.Location = New Point(208, 541)
        txtSelect.Margin = New Padding(3, 4, 3, 4)
        txtSelect.Name = "txtSelect"
        txtSelect.Size = New Size(547, 31)
        txtSelect.TabIndex = 8
        txtSelect.Text = "Select * From"
        ' 
        ' grbAccess
        ' 
        grbAccess.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        grbAccess.Controls.Add(Label4)
        grbAccess.Controls.Add(txtAccessPassword)
        grbAccess.Controls.Add(Label6)
        grbAccess.Controls.Add(txtAccessProvider)
        grbAccess.Controls.Add(btnExaminar)
        grbAccess.Controls.Add(txtAccessNombreBase)
        grbAccess.Controls.Add(Label8)
        grbAccess.FlatStyle = FlatStyle.System
        grbAccess.Location = New Point(22, 15)
        grbAccess.Margin = New Padding(3, 4, 3, 4)
        grbAccess.Name = "grbAccess"
        grbAccess.Padding = New Padding(3, 4, 3, 4)
        grbAccess.Size = New Size(1100, 189)
        grbAccess.TabIndex = 0
        grbAccess.TabStop = False
        ' 
        ' Label4
        ' 
        Label4.Location = New Point(23, 141)
        Label4.Name = "Label4"
        Label4.Size = New Size(178, 37)
        Label4.TabIndex = 5
        Label4.Text = "Password:"
        toolTip1.SetToolTip(Label4, " Si la base está protegida con contraseña, escíbela aquí ")
        ' 
        ' txtAccessPassword
        ' 
        txtAccessPassword.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        txtAccessPassword.Location = New Point(207, 138)
        txtAccessPassword.Margin = New Padding(3, 4, 3, 4)
        txtAccessPassword.Name = "txtAccessPassword"
        txtAccessPassword.Size = New Size(721, 31)
        txtAccessPassword.TabIndex = 6
        ' 
        ' Label6
        ' 
        Label6.Location = New Point(23, 97)
        Label6.Name = "Label6"
        Label6.Size = New Size(178, 37)
        Label6.TabIndex = 3
        Label6.Text = "Provider:"
        toolTip1.SetToolTip(Label6, " El proveedor de la base de datos (normalmente: Provider: (Microsoft.Jet.OLEDB.4.0) ")
        ' 
        ' txtAccessProvider
        ' 
        txtAccessProvider.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        txtAccessProvider.Location = New Point(207, 94)
        txtAccessProvider.Margin = New Padding(3, 4, 3, 4)
        txtAccessProvider.Name = "txtAccessProvider"
        txtAccessProvider.Size = New Size(721, 31)
        txtAccessProvider.TabIndex = 4
        ' 
        ' btnExaminar
        ' 
        btnExaminar.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        btnExaminar.FlatStyle = FlatStyle.System
        btnExaminar.Location = New Point(954, 38)
        btnExaminar.Margin = New Padding(3, 4, 3, 4)
        btnExaminar.Name = "btnExaminar"
        btnExaminar.Size = New Size(133, 42)
        btnExaminar.TabIndex = 2
        btnExaminar.Text = "&Examinar..."
        toolTip1.SetToolTip(btnExaminar, " Seleccionar la base de datos ")
        ' 
        ' txtAccessNombreBase
        ' 
        txtAccessNombreBase.AllowDrop = True
        txtAccessNombreBase.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        txtAccessNombreBase.Location = New Point(207, 44)
        txtAccessNombreBase.Margin = New Padding(3, 4, 3, 4)
        txtAccessNombreBase.Name = "txtAccessNombreBase"
        txtAccessNombreBase.Size = New Size(721, 31)
        txtAccessNombreBase.TabIndex = 1
        ' 
        ' Label8
        ' 
        Label8.Location = New Point(23, 47)
        Label8.Name = "Label8"
        Label8.Size = New Size(178, 37)
        Label8.TabIndex = 0
        Label8.Text = "Base de datos:"
        toolTip1.SetToolTip(Label8, " Base de datos ")
        ' 
        ' grbSQL
        ' 
        grbSQL.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        grbSQL.Controls.Add(chkUsarSQLEXpress)
        grbSQL.Controls.Add(chkSeguridadSQL)
        grbSQL.Controls.Add(txtSqlPassword)
        grbSQL.Controls.Add(txtSqlUserId)
        grbSQL.Controls.Add(txtSqlInitialCatalog)
        grbSQL.Controls.Add(txtSqlDataSource)
        grbSQL.Controls.Add(labelPassw)
        grbSQL.Controls.Add(labelUser)
        grbSQL.Controls.Add(Label2)
        grbSQL.Controls.Add(Label1)
        grbSQL.FlatStyle = FlatStyle.System
        grbSQL.Location = New Point(22, 212)
        grbSQL.Margin = New Padding(3, 4, 3, 4)
        grbSQL.Name = "grbSQL"
        grbSQL.Padding = New Padding(3, 4, 3, 4)
        grbSQL.Size = New Size(1100, 250)
        grbSQL.TabIndex = 2
        grbSQL.TabStop = False
        ' 
        ' chkUsarSQLEXpress
        ' 
        chkUsarSQLEXpress.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        chkUsarSQLEXpress.FlatStyle = FlatStyle.System
        chkUsarSQLEXpress.Location = New Point(910, -3)
        chkUsarSQLEXpress.Margin = New Padding(3, 4, 3, 4)
        chkUsarSQLEXpress.Name = "chkUsarSQLEXpress"
        chkUsarSQLEXpress.Size = New Size(184, 37)
        chkUsarSQLEXpress.TabIndex = 9
        chkUsarSQLEXpress.Text = "Usar SQLEXPRESS"
        toolTip1.SetToolTip(chkUsarSQLEXpress, "Marca esta casilla para usar '.\SQLEXPRESS' en el Data source y 'Seguridad Integrada'")
        ' 
        ' chkSeguridadSQL
        ' 
        chkSeguridadSQL.FlatStyle = FlatStyle.System
        chkSeguridadSQL.Location = New Point(22, 146)
        chkSeguridadSQL.Margin = New Padding(3, 4, 3, 4)
        chkSeguridadSQL.Name = "chkSeguridadSQL"
        chkSeguridadSQL.Size = New Size(712, 37)
        chkSeguridadSQL.TabIndex = 4
        chkSeguridadSQL.Text = "Usar seguridad de SQL, sino la seguridad integrada (Integrated Security) "
        toolTip1.SetToolTip(chkSeguridadSQL, " Marca esta opción para indicar el usuario y el password de la base de datos sino se usará la autenticación de Windows ")
        ' 
        ' txtSqlPassword
        ' 
        txtSqlPassword.Enabled = False
        txtSqlPassword.Location = New Point(527, 198)
        txtSqlPassword.Margin = New Padding(3, 4, 3, 4)
        txtSqlPassword.Name = "txtSqlPassword"
        txtSqlPassword.PasswordChar = "*"c
        txtSqlPassword.Size = New Size(184, 31)
        txtSqlPassword.TabIndex = 8
        ' 
        ' txtSqlUserId
        ' 
        txtSqlUserId.Enabled = False
        txtSqlUserId.Location = New Point(185, 198)
        txtSqlUserId.Margin = New Padding(3, 4, 3, 4)
        txtSqlUserId.Name = "txtSqlUserId"
        txtSqlUserId.Size = New Size(191, 31)
        txtSqlUserId.TabIndex = 6
        ' 
        ' txtSqlInitialCatalog
        ' 
        txtSqlInitialCatalog.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        txtSqlInitialCatalog.Location = New Point(207, 94)
        txtSqlInitialCatalog.Margin = New Padding(3, 4, 3, 4)
        txtSqlInitialCatalog.Name = "txtSqlInitialCatalog"
        txtSqlInitialCatalog.Size = New Size(721, 31)
        txtSqlInitialCatalog.TabIndex = 3
        txtSqlInitialCatalog.Text = "Downloads"
        ' 
        ' txtSqlDataSource
        ' 
        txtSqlDataSource.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        txtSqlDataSource.Location = New Point(207, 44)
        txtSqlDataSource.Margin = New Padding(3, 4, 3, 4)
        txtSqlDataSource.Name = "txtSqlDataSource"
        txtSqlDataSource.Size = New Size(721, 31)
        txtSqlDataSource.TabIndex = 1
        txtSqlDataSource.Text = "(local)\NETSDK"
        ' 
        ' labelPassw
        ' 
        labelPassw.Enabled = False
        labelPassw.Location = New Point(393, 201)
        labelPassw.Name = "labelPassw"
        labelPassw.Size = New Size(128, 37)
        labelPassw.TabIndex = 7
        labelPassw.Text = "Password:"
        toolTip1.SetToolTip(labelPassw, " La contraseña para la base de SQL Server ")
        ' 
        ' labelUser
        ' 
        labelUser.Enabled = False
        labelUser.Location = New Point(51, 201)
        labelUser.Name = "labelUser"
        labelUser.Size = New Size(128, 37)
        labelUser.TabIndex = 5
        labelUser.Text = "User id:"
        toolTip1.SetToolTip(labelUser, " El ID de usuario ")
        ' 
        ' Label2
        ' 
        Label2.Location = New Point(23, 97)
        Label2.Name = "Label2"
        Label2.Size = New Size(178, 37)
        Label2.TabIndex = 2
        Label2.Text = "Initial catalog:"
        toolTip1.SetToolTip(Label2, " La base de datos de SQL Server ")
        ' 
        ' Label1
        ' 
        Label1.Location = New Point(23, 47)
        Label1.Name = "Label1"
        Label1.Size = New Size(178, 37)
        Label1.TabIndex = 0
        Label1.Text = "Data source:"
        toolTip1.SetToolTip(Label1, " El servidor de SQL a utilizar ")
        ' 
        ' Panel1
        ' 
        Panel1.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        Panel1.Controls.Add(optCS)
        Panel1.Controls.Add(optVB)
        Panel1.Location = New Point(914, 50)
        Panel1.Margin = New Padding(3, 4, 3, 4)
        Panel1.Name = "Panel1"
        Panel1.Size = New Size(172, 44)
        Panel1.TabIndex = 6
        ' 
        ' optCS
        ' 
        optCS.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        optCS.FlatStyle = FlatStyle.System
        optCS.Location = New Point(92, 4)
        optCS.Margin = New Padding(3, 4, 3, 4)
        optCS.Name = "optCS"
        optCS.Size = New Size(67, 37)
        optCS.TabIndex = 1
        optCS.Text = "&C#"
        toolTip1.SetToolTip(optCS, " Para generar el código para C# ")
        ' 
        ' optVB
        ' 
        optVB.Checked = True
        optVB.FlatStyle = FlatStyle.System
        optVB.Location = New Point(10, 4)
        optVB.Margin = New Padding(3, 4, 3, 4)
        optVB.Name = "optVB"
        optVB.Size = New Size(67, 37)
        optVB.TabIndex = 0
        optVB.TabStop = True
        optVB.Text = "&VB"
        toolTip1.SetToolTip(optVB, " Para generar el código para VB .NET ")
        ' 
        ' optAccess
        ' 
        optAccess.FlatStyle = FlatStyle.System
        optAccess.Location = New Point(42, 8)
        optAccess.Margin = New Padding(3, 4, 3, 4)
        optAccess.Name = "optAccess"
        optAccess.Size = New Size(328, 44)
        optAccess.TabIndex = 1
        optAccess.Text = "Usar base de datos de &Access"
        toolTip1.SetToolTip(optAccess, " Para usar una base de Access (OleDb) " & vbCrLf & "(Se usará el DataAdapter)")
        ' 
        ' optSQL
        ' 
        optSQL.Checked = True
        optSQL.FlatStyle = FlatStyle.System
        optSQL.Location = New Point(42, 208)
        optSQL.Margin = New Padding(3, 4, 3, 4)
        optSQL.Name = "optSQL"
        optSQL.Size = New Size(370, 44)
        optSQL.TabIndex = 3
        optSQL.TabStop = True
        optSQL.Text = "Usar base de datos de &SQL Server"
        toolTip1.SetToolTip(optSQL, " Para usar una base de SQL Server " & vbCrLf & "(Se recomienda no usar el DataAdapter)")
        ' 
        ' chkUsarCommandBuilder
        ' 
        chkUsarCommandBuilder.Checked = True
        chkUsarCommandBuilder.CheckState = CheckState.Checked
        chkUsarCommandBuilder.FlatStyle = FlatStyle.System
        chkUsarCommandBuilder.Location = New Point(40, 63)
        chkUsarCommandBuilder.Margin = New Padding(3, 4, 3, 4)
        chkUsarCommandBuilder.Name = "chkUsarCommandBuilder"
        chkUsarCommandBuilder.Size = New Size(222, 37)
        chkUsarCommandBuilder.TabIndex = 1
        chkUsarCommandBuilder.Text = "usar CommandBuilder"
        toolTip1.SetToolTip(chkUsarCommandBuilder, " Si se debe usar CommandBuilder para generar el código UPDATE, INSERT o DELETE ")
        ' 
        ' chkUsarAddWithValue
        ' 
        chkUsarAddWithValue.Checked = True
        chkUsarAddWithValue.CheckState = CheckState.Checked
        chkUsarAddWithValue.FlatStyle = FlatStyle.System
        chkUsarAddWithValue.Location = New Point(40, 108)
        chkUsarAddWithValue.Margin = New Padding(3, 4, 3, 4)
        chkUsarAddWithValue.Name = "chkUsarAddWithValue"
        chkUsarAddWithValue.Size = New Size(210, 37)
        chkUsarAddWithValue.TabIndex = 2
        chkUsarAddWithValue.Text = "usar AddWithValue"
        toolTip1.SetToolTip(chkUsarAddWithValue, " Si se utiliza AddWithValue o Add  para añadir los valores a los parámetros del comando (cuando se usa DataAdapter).")
        ' 
        ' chkUsarDataAdapter
        ' 
        chkUsarDataAdapter.AutoSize = True
        chkUsarDataAdapter.Checked = True
        chkUsarDataAdapter.CheckState = CheckState.Checked
        chkUsarDataAdapter.Location = New Point(10, 23)
        chkUsarDataAdapter.Margin = New Padding(3, 4, 3, 4)
        chkUsarDataAdapter.Name = "chkUsarDataAdapter"
        chkUsarDataAdapter.Size = New Size(395, 29)
        chkUsarDataAdapter.TabIndex = 0
        chkUsarDataAdapter.Text = "usar DataAdapater en lugar de cmd.Execute..."
        toolTip1.SetToolTip(chkUsarDataAdapter, "Marca esta opción si quieres usar DataAdapter en vez de ExecuteNonQuery en UPDATE y DELETE o ExecuteScalar en INSERT." & vbCrLf & "(Al usar cmd.Execute en SQL se usarán transacciones)")
        chkUsarDataAdapter.UseVisualStyleBackColor = False
        ' 
        ' chkUsarOverrides
        ' 
        chkUsarOverrides.Checked = True
        chkUsarOverrides.CheckState = CheckState.Checked
        chkUsarOverrides.FlatStyle = FlatStyle.System
        chkUsarOverrides.Location = New Point(442, 19)
        chkUsarOverrides.Margin = New Padding(3, 4, 3, 4)
        chkUsarOverrides.Name = "chkUsarOverrides"
        chkUsarOverrides.Size = New Size(210, 37)
        chkUsarOverrides.TabIndex = 3
        chkUsarOverrides.Text = "usar Overrides"
        toolTip1.SetToolTip(chkUsarOverrides, "Usar Overrides en los métodos Actualizar, Crear y Borrar.")
        ' 
        ' chkPropiedadAuto
        ' 
        chkPropiedadAuto.AutoSize = True
        chkPropiedadAuto.Checked = True
        chkPropiedadAuto.CheckState = CheckState.Checked
        chkPropiedadAuto.FlatStyle = FlatStyle.System
        chkPropiedadAuto.Location = New Point(442, 64)
        chkPropiedadAuto.Margin = New Padding(3, 4, 3, 4)
        chkPropiedadAuto.Name = "chkPropiedadAuto"
        chkPropiedadAuto.Size = New Size(319, 30)
        chkPropiedadAuto.TabIndex = 4
        chkPropiedadAuto.Text = "Propiedades auto-implementadas"
        toolTip1.SetToolTip(chkPropiedadAuto, "Cuando el tipo no es String usar propiedades auto-implementadas (sin cuerpo get/set)")
        ' 
        ' chkCrearIndizador
        ' 
        chkCrearIndizador.AutoSize = True
        chkCrearIndizador.FlatStyle = FlatStyle.System
        chkCrearIndizador.Location = New Point(442, 102)
        chkCrearIndizador.Margin = New Padding(3, 4, 3, 4)
        chkCrearIndizador.Name = "chkCrearIndizador"
        chkCrearIndizador.Size = New Size(315, 30)
        chkCrearIndizador.TabIndex = 5
        chkCrearIndizador.Text = "Crear indizador (Default Property)"
        toolTip1.SetToolTip(chkCrearIndizador, "Si se debe crear la propiedad predeterminada (Item en VB) o indizador (C#)")
        ' 
        ' grbOpciones
        ' 
        grbOpciones.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        grbOpciones.Controls.Add(chkCrearIndizador)
        grbOpciones.Controls.Add(chkPropiedadAuto)
        grbOpciones.Controls.Add(chkUsarCommandBuilder)
        grbOpciones.Controls.Add(chkUsarOverrides)
        grbOpciones.Controls.Add(chkUsarAddWithValue)
        grbOpciones.Controls.Add(chkUsarDataAdapter)
        grbOpciones.Controls.Add(Panel1)
        grbOpciones.Controls.Add(btnGenerarClase)
        grbOpciones.Location = New Point(23, 633)
        grbOpciones.Margin = New Padding(5, 6, 5, 6)
        grbOpciones.Name = "grbOpciones"
        grbOpciones.Padding = New Padding(5, 6, 5, 6)
        grbOpciones.Size = New Size(1094, 154)
        grbOpciones.TabIndex = 11
        grbOpciones.TabStop = False
        ' 
        ' StatusStrip1
        ' 
        StatusStrip1.ImageScalingSize = New Size(24, 24)
        StatusStrip1.Items.AddRange(New ToolStripItem() {LabelInfo, LabelVersion})
        StatusStrip1.Location = New Point(0, 1298)
        StatusStrip1.Name = "StatusStrip1"
        StatusStrip1.Padding = New Padding(2, 0, 23, 0)
        StatusStrip1.Size = New Size(1148, 36)
        StatusStrip1.TabIndex = 15
        StatusStrip1.Text = "StatusStrip1"
        ' 
        ' LabelInfo
        ' 
        LabelInfo.DisplayStyle = ToolStripItemDisplayStyle.Text
        LabelInfo.Name = "LabelInfo"
        LabelInfo.Size = New Size(993, 29)
        LabelInfo.Spring = True
        LabelInfo.Text = "©Guillermo Som (elGuille), 2004-2007, 2018-2022"
        LabelInfo.TextAlign = ContentAlignment.MiddleLeft
        ' 
        ' LabelVersion
        ' 
        LabelVersion.BorderSides = ToolStripStatusLabelBorderSides.Left
        LabelVersion.DisplayStyle = ToolStripItemDisplayStyle.Text
        LabelVersion.Name = "LabelVersion"
        LabelVersion.Size = New Size(130, 29)
        LabelVersion.Text = "v3.0.0 (3.0.0.0)"
        ' 
        ' LabelCodigo
        ' 
        LabelCodigo.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        LabelCodigo.BorderStyle = BorderStyle.Fixed3D
        LabelCodigo.Location = New Point(938, 797)
        LabelCodigo.Name = "LabelCodigo"
        LabelCodigo.Size = New Size(192, 37)
        LabelCodigo.TabIndex = 16
        ' 
        ' Form1
        ' 
        AllowDrop = True
        AutoScaleDimensions = New SizeF(10F, 25F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(1148, 1334)
        Controls.Add(LabelCodigo)
        Controls.Add(StatusStrip1)
        Controls.Add(grbOpciones)
        Controls.Add(optSQL)
        Controls.Add(txtSelect)
        Controls.Add(txtCodigo)
        Controls.Add(txtClase)
        Controls.Add(optAccess)
        Controls.Add(Label7)
        Controls.Add(btnGuardar)
        Controls.Add(cboTablas)
        Controls.Add(Label3)
        Controls.Add(btnMostrarTablas)
        Controls.Add(Label5)
        Controls.Add(btnSalir)
        Controls.Add(grbAccess)
        Controls.Add(grbSQL)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        Margin = New Padding(3, 4, 3, 4)
        MinimumSize = New Size(975, 1016)
        Name = "Form1"
        StartPosition = FormStartPosition.CenterScreen
        Text = "Crear clases de una base de datos Access o SQL Server"
        grbAccess.ResumeLayout(False)
        grbAccess.PerformLayout()
        grbSQL.ResumeLayout(False)
        grbSQL.PerformLayout()
        Panel1.ResumeLayout(False)
        grbOpciones.ResumeLayout(False)
        grbOpciones.PerformLayout()
        StatusStrip1.ResumeLayout(False)
        StatusStrip1.PerformLayout()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Private WithEvents btnGuardar As System.Windows.Forms.Button
    Private WithEvents cboTablas As System.Windows.Forms.ComboBox
    Private WithEvents Label3 As System.Windows.Forms.Label
    Private WithEvents txtCodigo As System.Windows.Forms.TextBox
    Private WithEvents btnGenerarClase As System.Windows.Forms.Button
    Private WithEvents Label5 As System.Windows.Forms.Label
    Private WithEvents txtClase As System.Windows.Forms.TextBox
    Private WithEvents btnSalir As System.Windows.Forms.Button
    Private WithEvents Label7 As System.Windows.Forms.Label
    Private WithEvents txtSelect As System.Windows.Forms.TextBox
    Private WithEvents btnMostrarTablas As System.Windows.Forms.Button
    Private WithEvents Label4 As System.Windows.Forms.Label
    Private WithEvents Label6 As System.Windows.Forms.Label
    Private WithEvents btnExaminar As System.Windows.Forms.Button
    Private WithEvents txtAccessNombreBase As System.Windows.Forms.TextBox
    Private WithEvents Label8 As System.Windows.Forms.Label
    Private WithEvents Panel1 As System.Windows.Forms.Panel
    Private WithEvents optCS As System.Windows.Forms.RadioButton
    Private WithEvents optVB As System.Windows.Forms.RadioButton
    Private WithEvents chkSeguridadSQL As System.Windows.Forms.CheckBox
    Private WithEvents txtSqlPassword As System.Windows.Forms.TextBox
    Private WithEvents txtSqlUserId As System.Windows.Forms.TextBox
    Private WithEvents txtSqlInitialCatalog As System.Windows.Forms.TextBox
    Private WithEvents txtSqlDataSource As System.Windows.Forms.TextBox
    Private WithEvents labelPassw As System.Windows.Forms.Label
    Private WithEvents labelUser As System.Windows.Forms.Label
    Private WithEvents Label2 As System.Windows.Forms.Label
    Private WithEvents Label1 As System.Windows.Forms.Label
    Private WithEvents optAccess As System.Windows.Forms.RadioButton
    Private WithEvents optSQL As System.Windows.Forms.RadioButton
    Private WithEvents grbAccess As System.Windows.Forms.GroupBox
    Private WithEvents grbSQL As System.Windows.Forms.GroupBox
    Private WithEvents txtAccessPassword As System.Windows.Forms.TextBox
    Private WithEvents txtAccessProvider As System.Windows.Forms.TextBox
    Private WithEvents toolTip1 As System.Windows.Forms.ToolTip
    Private WithEvents chkUsarCommandBuilder As System.Windows.Forms.CheckBox
    Private WithEvents chkUsarAddWithValue As CheckBox
    Private WithEvents chkUsarDataAdapter As CheckBox
    Private WithEvents chkUsarOverrides As CheckBox
    Friend WithEvents grbOpciones As GroupBox
    Private WithEvents chkUsarSQLEXpress As CheckBox
    Friend WithEvents StatusStrip1 As StatusStrip
    Private WithEvents LabelInfo As ToolStripStatusLabel
    Friend WithEvents LabelVersion As ToolStripStatusLabel
    Private WithEvents chkPropiedadAuto As CheckBox
    Private WithEvents chkCrearIndizador As CheckBox
    Private WithEvents LabelCodigo As Label
End Class
