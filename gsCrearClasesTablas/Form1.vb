'------------------------------------------------------------------------------
' Utilidad para generar clases basadas en tablas de bases de datos  (07/Jul/04)
'
' Basada en crearClasesVB                                           (21/Mar/04)
'
' Para Visual Basic 2005 usando elGuille.Util.Developer.Data        (17/Abr/07)
'
' Quitar de gitHub los ficheros elguille.snk                    (01/oct/22 12.52)
'
' ©Guillermo Som (elGuille), 2004-2005, 2007, 2018-2019, 2021-2023
'------------------------------------------------------------------------------
' Revisiones:
'   0.0000  07/Jul/2004 Empiezo con los cambios para usar tablas SQL
'                       Un solo método para crear tanto código de VB como de C#
'                       que se apoya en la clase ConvLang
'           08/Jul/2004 Cambio bastante en el código generado
'           08/Jul/2004 Creo una clase para la conexión y generación del código
'           10/Jul/2004 Mejoras en la creación de la clase
'           13/Jul/2004 La misma aplicación para SQL y OleDb
'                       Las clases de creación están en una librería aparte
'   0.1000  14/Jul/2004 Publico la utilidad
'   0.1010  02/Nov/2004 Si el nombre de la clase contiene espacios,
'                       sustituirlos por un guión bajo  (David Sans)
'   0.1030  07/Feb/2005 Errores al borrar un registro y al cambiar de tabla
'   0.1040  15/Mar/2005 En el password ahora se muestra *
'
'   1.0000  17/Nov/2018 Añado a la clase CrearClase de la DLL DeveloperData:
'                       Para generar Option Infer On y Imports Microsoft.VisualBasic
'   1.0001  20/Nov/2018 Bug al añadir Imports Microsoft.VisualBasic
'                       (añadía dos veces Imports)
'   2.0000  30/Nov/2018 Utilizo el .NET 4.7.2
'
'   2.0001  15/Dic/2018 Utilizo ConversorTipos para convertir de cadena a un tipo
'                       Los tipos convertidos son:
'                       Int16, Int32, Int64, Single, Decimal, Double, Byte, SByte,
'                       UInt16, UInt32, UInt64, Boolean, DateTime, Char, TimeSpan
'   2.0002  19/Mar/2019 Añado la opción AddWithValue a los comandos...
'   2.0003  20/Mar/2019 El método Borrar se utiliza con Command en vez de DataAdapter
'                       En convLang añado Using/End Using
'   2.0004  22/Mar/2019 Algunos cambios menores en las clases aparte el de añadir
'                       la opción de poder usar ExecuteScalar en lugar de DataAdapter
'                       No quito el uso de DataAdapter en INSERT y UPDATE
'                       si no que se puede usar ExecuteScalar o un DataAdapter.
'
'   2.0016  17/abr/2021 Poder asignar rápidamente el uso de SQLExpress.
'           02/oct/2021 Seguramente lo cambié a .NET Framework 4.8
'
'   2.1.0   01/oct/2022 Cambio a .NET Framework 4.8.1 y cambios en CrearClase.
'   2.1.0.7 01/oct/2022 Añado un StatusStrip
'   2.1.0.8             Creo el fichero elGuille_compartido.snk para firmar los ensamblados.
'
'   3.0.0   01/oct/2022 Proyecto creado con .NET 6.0
'   3.0.0.1 05/oct/2022 Ajustes en los tamaños.
'   3.0.0.2             El valor fijo de VersionDLL.
'   3.0.1.0             Muevo las clases CrearClaseSQL y CrearClasesOleDb a este proyecto.
'   3.0.1.1             Quito los ByVal.
'   3.0.2.0 06/oct/2022 Usando la DLL con el código en C#.
'   3.0.3.0 08/oct/2022 Guardar los datos en la configuración local (como en móvil)
'   3.0.3.2             Habilitar el grupo de grbOpciones según haya tablas en la lista
'   3.0.5.0 10/oct/2022 Opción para las propiedades auto-implementadas.
'   3.0.6.0 11/oct/2022 Usando la versión 3.0.8 de gsCrearClases_CS
'   3.0.6.2             chkUsarAddWithValue solo se usa con chkUsarDataAdapter
'   3.0.6.3             Usando la versión 3.0.9 de gsCrearClases_CS
'   3.0.6.4             Usando la versión 3.0.10 de gsCrearClases_CS
'   3.0.6.5~6           Usando la versión 3.0.11 de gsCrearClases_CS
'   3.0.7.0             Usando la versión 3.0.12 de gsCrearClases_CS
'   3.0.8.0 12/oct/2022 Mostrar generando y el tiempo empleado
'   3.0.8.2 13/oct/2022 Alinear la etiqueta de generando
'
'   3.0.8.4 14/may/2023 Indicar si se crea el indizador
'   3.0.8.5             No guardar todos los valores de configuración
'                       si se llama desde mostrar tablas
'                       LeerConfig para no depender de settings.
'   3.0.9.0             Para 2023 y los cambios en CrearClases.
'   3.1.0.0             Quito <MyType>WindowsForms</MyType> y Application.*.
'------------------------------------------------------------------------------
Option Strict On
Option Explicit On
Option Infer On
'
Imports System
Imports System.Windows.Forms
Imports Microsoft.VisualBasic

Imports elGuille.Util.Developer
Imports elGuille.Util.Developer.Data
Imports System.IO

Public Class Form1

    ' Para la utilidad de copiar.                           (14/may/23 13.29)

    ' Para los texto, etc. al hacer la copia con este programa.
    ' p-u-blic static string AppFileVersion { get; } = "8.8.1.0";
    Private Const AppFileVersion As String = "3.1.0.0"
    ' p-u-blic static string AppFechaVersion { get; } = "26-abr-2023";
    Private Const AppFechaVersion As String = "14-may-2023"
    ' Intentar no pasar de estas marcas: 60 caracteres.
    '                                            10        20        30        40        50        60
    '                                   ---------|---------|---------|---------|---------|---------|
    ' //[COPIAR]AppDescripcionCopia = " compilar sin usar My"

    Private Shared Property VersionInfo As FileVersionInfo

    ''' <summary>
    ''' Constructor compartido.
    ''' </summary>
    Shared Sub New()
        Try
            Dim ensamblado = GetType(Form1).Assembly
            VersionInfo = FileVersionInfo.GetVersionInfo(ensamblado.Location)
        Catch ex As Exception
            Debug.WriteLine(ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' El path local de la aplicación.
    ''' </summary>
    Private Property FolderPath As String
    ''' <summary>
    ''' El fichero de configuración.
    ''' </summary>
    Private Property FicConfig As String

    ''' <summary>
    ''' Devuelve el valor de FileVersion usando FileVersionInfo.
    ''' </summary>
    ''' <returns>El valor de FileVersion.</returns>
    ''' <remarks>01/Oct/22</remarks>
    Private Shared Function VersionDLL() As String
        Dim s As String = AppFileVersion

        Try
            'Dim ensamblado = GetType(Form1).Assembly
            ''Dim ensamblado = System.Reflection.Assembly.GetExecutingAssembly()
            'Dim fvi = FileVersionInfo.GetVersionInfo(ensamblado.Location)
            '' FileDescription en realidad muestra (o eso parece) lo mismo de ProductName
            's = fvi.FileVersion
            If VersionInfo IsNot Nothing Then
                s = VersionInfo.FileVersion
            End If
        Catch ex As Exception
        End Try

        Return s
    End Function

    Private ValoresAntSQLExpress As (usarSQL As Boolean, segIntegrada As Boolean, dataSource As String, initialCatalog As String)

    Private inicializando As Boolean = True

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim ts As TimeSpan = Nothing

        FolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData))
        FicConfig = Path.Combine(FolderPath, "gsCrearClasesTablas.txt")

        ' Quito todo lo relacionado con My.Sttings          (14/may/23 09.38)
        ' Salvo para el tamaño y posición de la ventana.
        ' Actualizar la versión de los settings para no perder los valores anteriores. (05/oct/22 12.17)
        My.Settings.Upgrade()
        My.Settings.Save()

        ' centrar el formulario horizontalmente
        Me.Left = (Screen.PrimaryScreen.Bounds.Width - Me.Width) \ 2

        ' Solo asignar los valores guardados si no es un valor negativo (20/Dic/20)
        If My.Settings.Left <> -1 Then
            If My.Settings.Left > -1 Then
                Me.Left = My.Settings.Left
                Me.Top = My.Settings.Top
            End If
        End If
        ' Añdir información al status. (01/oct/22 11.55)
        Dim sCopyR = "©Guillermo Som (elGuille), 2004-2007, 2018-"
        Dim elAño = 2023
        If Date.Today.Year > elAño Then
            elAño = Date.Today.Year
        End If
        LabelInfo.Text = $"  {sCopyR}{elAño}  "
        ' Usando My.Application.Info.Version.ToString(3) devuelve solo las 3 primeras cifras.
        'LabelVersion.Text = $"  {My.Application.Info.ProductName} - v{My.Application.Info.Version.ToString(3)} ({VersionDLL()})  "
        LabelVersion.Text = $"  {VersionInfo.ProductName} - v{VersionInfo.ProductVersion} ({VersionDLL()})  "

        txtSelect.Text = ""
        txtCodigo.Text = ""
        txtClase.Text = ""

        LeerConfig()

        'txtDataSource.Text = My.Settings.SQLDataSource
        'txtInitialCatalog.Text = My.Settings.SQLInitialCatalog
        'chkSeguridadSQL.Checked = My.Settings.SQLSeguridad
        'txtUserId.Text = My.Settings.SQLUserId
        'txtPassword.Text = My.Settings.SQLPassword

        'txtNombreBase.Text = My.Settings.OleDbBaseDeDatos
        'txtProvider.Text = My.Settings.OleDbProvider
        'txtAccessPassword.Text = My.Settings.OleDbPassword

        'If My.Settings.UsarSQL Then
        '    optAccess.Checked = False
        '    optSQL.Checked = True
        'Else
        '    optSQL.Checked = False
        '    optAccess.Checked = True
        'End If
        grbSQL.Enabled = optSQL.Checked
        grbAccess.Enabled = optAccess.Checked

        'If My.Settings.Lenguaje = "VB" Then
        '    optCS.Checked = False
        '    optVB.Checked = True
        'Else
        '    optVB.Checked = False
        '    optCS.Checked = True
        'End If

        ' Guardar los valores iniciales, por si se usa chkUsarSQLEXpress (17/Abr/21)
        ValoresAntSQLExpress = (optSQL.Checked, chkSeguridadSQL.Checked, txtSqlDataSource.Text, txtSqlInitialCatalog.Text)

        inicializando = False

        'chkUsarCommandBuilder.Checked = My.Settings.UsarCommandBuilder
        'chkUsarAddWithValue.Checked = My.Settings.UsarAddWithValue
        'chkUsarDataAdapter.Checked = My.Settings.UsarExecuteScalar
        'chkUsarOverrides.Checked = My.Settings.usarOverrides
        '' Opción para usar las propiedades auto-implementadas. (10/oct/22 19.30)
        'chkPropiedadAuto.Checked = My.Settings.PropiedadAuto
        '' Opción para crear el indizador/default property. (11/oct/22 22.58)
        'chkCrearIndizador.Checked = My.Settings.CrearIndizador

        btnGuardar.Enabled = False
        grbOpciones.Enabled = btnGuardar.Enabled
        btnGenerarClase.Enabled = False
        Panel1.Enabled = False

        chkUsarCommandBuilder.Enabled = chkUsarDataAdapter.Checked
        chkUsarAddWithValue.Enabled = chkUsarDataAdapter.Checked
    End Sub
    '
    Private Sub Form1_Closing(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles MyBase.Closing
        ' Guardar todos los valores.                        (14/may/23 08.50)
        ' Cuando está depurando, si se hace parada          (14/may/23 09.06)
        ' en el código desde esta llamada, da error.
        Try
            guardarCfg(soloConexion:=False)
        Catch ex As Exception
            Debug.WriteLine(ex.Message)
        End Try
    End Sub
    '
    Private Sub btnSalir_Click(sender As Object, e As EventArgs) Handles btnSalir.Click
        Close()
    End Sub
    '
    Private Sub cboTablas_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboTablas.SelectedIndexChanged
        txtSelect.Text = "SELECT * FROM " & cboTablas.Text
        Dim i As Integer = cboTablas.Text.IndexOf(".")
        ' Si la tabla contiene espacios,                            (02/Nov/04)
        ' sustituirlos por guiones bajos.
        ' Bug reportado por David Sans
        If i > -1 Then
            txtClase.Text = cboTablas.Text.Substring(i + 1).Replace(" ", "_")
        Else
            txtClase.Text = cboTablas.Text.Replace(" ", "_")
        End If
    End Sub
    '
    Private Sub btnMostrarTablas_Click(sender As Object, e As EventArgs) Handles btnMostrarTablas.Click
        ' Solo asignar los datos de conexión, etc.          (14/may/23 08.47)
        guardarCfg(soloConexion:=True)

        ' No tener en cuenta la cadena select para mostrar las tablas
        txtSelect.Text = ""
        If optSQL.Checked Then
            CrearClaseSQL.Conectar(txtSqlDataSource.Text, txtSqlInitialCatalog.Text, txtSelect.Text, txtSqlUserId.Text, txtSqlPassword.Text, chkSeguridadSQL.Checked)
        Else
            CrearClaseOleDb.Conectar(Me.txtAccessNombreBase.Text, txtSelect.Text, txtAccessProvider.Text, txtAccessPassword.Text)
        End If

        btnGuardar.Enabled = CrearClase.Conectado
        grbOpciones.Enabled = btnGuardar.Enabled
        btnGenerarClase.Enabled = CrearClase.Conectado
        Panel1.Enabled = CrearClase.Conectado

        If CrearClase.Conectado = False Then Return

        Dim nomTablas As List(Of String)
        If optSQL.Checked Then
            ' Esto necesita usar una importación a elGuille.Util.Developer.Data
            nomTablas = CrearClaseSQL.NombresTablas()
        Else
            nomTablas = CrearClaseOleDb.NombresTablas()
        End If

        If (nomTablas Is Nothing) OrElse nomTablas(0).StartsWith("ERROR") Then
            btnGuardar.Enabled = False
            grbOpciones.Enabled = btnGuardar.Enabled
            btnGenerarClase.Enabled = False
            Panel1.Enabled = False
            'Return
        End If

        cboTablas.Items.Clear()
        If nomTablas IsNot Nothing Then
            'For i As Integer = 0 To nomTablas.Length - 1
            For i As Integer = 0 To nomTablas.Count - 1
                cboTablas.Items.Add(nomTablas(i))
            Next
        End If
        If cboTablas.Items.Count > 0 Then
            cboTablas.SelectedIndex = 0
        End If

        chkUsarCommandBuilder.Enabled = chkUsarDataAdapter.Checked
        chkUsarAddWithValue.Enabled = chkUsarDataAdapter.Checked
    End Sub
    '
    Private Async Sub btnGenerarClase_Click(sender As Object, e As EventArgs) Handles btnGenerarClase.Click
        Dim sw = Stopwatch.StartNew()

        LabelCodigo.Text = "Generando..."
        LabelCodigo.BackColor = Color.MediumTurquoise
        LabelCodigo.ForeColor = Color.White
        ' El tamaño es con dpi
        Dim dpi = DeviceDpi / 144
        'LabelCodigo.Size = New Size(192, 37)
        LabelCodigo.Size = New Size(CInt(192 * dpi), CInt(37 * dpi))

        Await Task.Delay(10)

        ' generar la clase a partir de la tabla seleccionada
        If txtSelect.Text = "" Then
            MessageBox.Show("Debes especificar la cadena de selección de datos",
                            "Generar clase",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            txtSelect.Focus()
            Return
        End If

        txtCodigo.Text = ""
        ' Guardar todos los valores.                        (14/may/23 08.50)
        guardarCfg(soloConexion:=False)
        '
        ' Si la tabla contiene espacios,                            (02/Nov/04)
        ' sustituirlos por guiones bajos.
        ' Bug reportado por David Sans
        txtClase.Text = txtClase.Text.Replace(" ", "_")

        ' Lo asigno en la clase base ya que son métodos compartidos (22/Mar/19)
        CrearClase.UsarDataAdapter = chkUsarDataAdapter.Checked
        'If chkUsarDataAdapter.Checked = False Then
        '    chkUsarCommandBuilder.Enabled = False
        '    chkUsarAddWithValue.Enabled = False
        'End If
        chkUsarCommandBuilder.Enabled = chkUsarDataAdapter.Checked
        chkUsarAddWithValue.Enabled = chkUsarDataAdapter.Checked

        CrearClase.UsarAddWithValue = chkUsarAddWithValue.Checked
        CrearClase.UsarOverrides = chkUsarOverrides.Checked
        ' Opción para usar las propiedades auto-implementadas. (10/oct/22 19.34)
        CrearClase.PropiedadAuto = chkPropiedadAuto.Checked

        ' Si se debe crear el indizador.                    (14/may/23 08.33)
        CrearClase.CrearIndizador = chkCrearIndizador.Checked

        ' Si no está habilitado es que no se utiliza                (07/Abr/19)
        ' ya que solo se usa con DataAdapter
        Dim usarCB As Boolean = chkUsarCommandBuilder.Checked
        If chkUsarCommandBuilder.Enabled = False Then
            'chkUsarCommandBuilder.Checked = False
            usarCB = False
        End If
        If optVB.Checked Then
            If optSQL.Checked Then
                txtCodigo.Text = CrearClaseSQL.GenerarClase(Lenguajes.eVBNET, usarCB,
                                                            txtClase.Text, cboTablas.Text, txtSqlDataSource.Text,
                                                            txtSqlInitialCatalog.Text, txtSelect.Text, txtSqlUserId.Text,
                                                            txtSqlPassword.Text, chkSeguridadSQL.Checked)
            Else
                txtCodigo.Text = CrearClaseOleDb.GenerarClase(Lenguajes.eVBNET, usarCB,
                                                              txtClase.Text, cboTablas.Text, txtAccessNombreBase.Text,
                                                              txtSelect.Text, txtAccessPassword.Text, txtAccessProvider.Text)
            End If
        Else
            If optSQL.Checked Then
                txtCodigo.Text = CrearClaseSQL.GenerarClase(Lenguajes.eCS, usarCB,
                                                            txtClase.Text, cboTablas.Text, txtSqlDataSource.Text,
                                                            txtSqlInitialCatalog.Text, txtSelect.Text, txtSqlUserId.Text,
                                                            txtSqlPassword.Text, chkSeguridadSQL.Checked)
            Else
                txtCodigo.Text = CrearClaseOleDb.GenerarClase(Lenguajes.eCS, usarCB,
                                                              txtClase.Text, cboTablas.Text, txtAccessNombreBase.Text,
                                                              txtSelect.Text, txtAccessPassword.Text, txtAccessProvider.Text)
            End If
        End If

        sw.Stop()

        LabelCodigo.Text = $"Código generado en {sw.Elapsed.TotalSeconds:#,##0.####} segundos"
        LabelCodigo.BackColor = Color.FromKnownColor(KnownColor.Control)
        LabelCodigo.ForeColor = Color.FromKnownColor(KnownColor.ControlText)
        LabelCodigo.Size = New Size(CInt(192 * dpi), CInt(75 * dpi))

    End Sub
    '
    Private Sub btnGuardar_Click(sender As Object, e As EventArgs) Handles btnGuardar.Click
        ' guarda la clase en un fichero .vb o .cs
        Dim sFic As String = Application.StartupPath & "\" & txtClase.Text
        If optVB.Checked Then
            sFic &= ".vb"
        Else
            sFic &= ".cs"
        End If
        Dim sw As New System.IO.StreamWriter(sFic, False, System.Text.Encoding.Default)
        sw.Write(txtCodigo.Text)
        sw.Close()
    End Sub
    '
    Private Sub chkSeguridadSQL_CheckedChanged(sender As Object, e As EventArgs) Handles chkSeguridadSQL.CheckedChanged
        Dim b As Boolean = chkSeguridadSQL.Checked
        labelUser.Enabled = b
        labelPassw.Enabled = b
        txtSqlUserId.Enabled = b
        txtSqlPassword.Enabled = b
    End Sub
    '
    Private Sub optAccess_CheckedChanged(sender As Object, e As EventArgs) Handles optAccess.CheckedChanged
        If inicializando Then Return
        grbAccess.Enabled = optAccess.Checked
        ' Con Access usar DataAdapter
        If optAccess.Checked Then
            chkUsarDataAdapter.Checked = True
        End If
    End Sub
    Private Sub optSQL_CheckedChanged(sender As Object, e As EventArgs) Handles optSQL.CheckedChanged
        If inicializando Then Return
        grbSQL.Enabled = optSQL.Checked
        ' Al cambiar a SQL seleccionar automáticamente Execute... es decir, desmarcar DataAdapter
        If optSQL.Checked Then
            chkUsarDataAdapter.Checked = False
        End If
    End Sub
    '
    Private Sub txtNombreBase_DragOver(
                    sender As Object,
                    e As System.Windows.Forms.DragEventArgs) _
                    Handles txtAccessNombreBase.DragOver, MyBase.DragOver
        e.Effect = DragDropEffects.Copy
    End Sub
    '
    Private Sub txtNombreBase_DragDrop(
                    sender As Object,
                    e As System.Windows.Forms.DragEventArgs) _
                    Handles txtAccessNombreBase.DragDrop, MyBase.DragDrop
        Dim archivos() As String
        '
        If e.Data.GetDataPresent("FileDrop") Then
            archivos = CType(e.Data.GetData("FileDrop"), String())
            txtAccessNombreBase.Text = archivos(0)
            txtAccessNombreBase.SelectionStart = txtAccessNombreBase.Text.Length
        End If
    End Sub
    '
    Private Sub btnExaminar_Click(sender As Object, e As EventArgs) Handles btnExaminar.Click
        With New OpenFileDialog
            .Title = "Seleccionar base de datos"
            .Filter = "Bases de Access (*.mdb)|*.mdb"
            .FileName = txtAccessNombreBase.Text
            If .ShowDialog = DialogResult.OK Then
                txtAccessNombreBase.Text = .FileName
            End If
        End With
    End Sub

    ''' <summary>
    ''' Guardar los datos de configuración.
    ''' </summary>
    ''' <param name="soloConexion">True para solo los de la conexión, etc.</param>
    Private Sub guardarCfg(soloConexion As Boolean)
        If WindowState = FormWindowState.Normal Then
            My.Settings.Left = Me.Left
            My.Settings.Top = Me.Top
        Else
            My.Settings.Left = Me.RestoreBounds.Left
            My.Settings.Top = Me.RestoreBounds.Top
        End If

        My.Settings.CopyrightAutor = "Guillermo Som (elGuille)"
        'My.Settings.CopyrightVersion = My.Application.Info.Version.ToString
        My.Settings.CopyrightVersion = Form1.VersionInfo.FileVersion

        ' Por si no está puesto que se guarde.                  (14/may/23 09.04)
        My.Settings.Save()

        ' Guardar los valore por compatibilidad con la app móvil (.NET MAUI)
        GuardarConfig()
    End Sub

    Private Sub chkUsarDataAdapter_CheckedChanged(sender As Object, e As EventArgs) Handles chkUsarDataAdapter.CheckedChanged
        If inicializando Then Return
        chkUsarCommandBuilder.Enabled = chkUsarDataAdapter.Checked
        ' Esto solo se usa con UsarDataAdapter. (11/oct/22 21.57)
        chkUsarAddWithValue.Enabled = chkUsarDataAdapter.Checked
    End Sub

    Private Sub chkUsarSQLEXpress_CheckedChanged(sender As Object, e As EventArgs) Handles chkUsarSQLEXpress.CheckedChanged
        If inicializando Then Return
        If chkUsarSQLEXpress.Checked Then
            ValoresAntSQLExpress = (optSQL.Checked, chkSeguridadSQL.Checked, txtSqlDataSource.Text, txtSqlInitialCatalog.Text)
            optSQL.Checked = True
            chkSeguridadSQL.Checked = False
            txtSqlDataSource.Text = ".\SQLEXPRESS"
        Else
            optSQL.Checked = ValoresAntSQLExpress.usarSQL
            chkSeguridadSQL.Checked = ValoresAntSQLExpress.segIntegrada
            txtSqlDataSource.Text = ValoresAntSQLExpress.dataSource
            txtSqlInitialCatalog.Text = ValoresAntSQLExpress.initialCatalog
        End If
    End Sub

    ''' <summary>
    ''' Guardar los datos de configuración.
    ''' </summary>
    Private Sub GuardarConfig()
        Using sw = New StreamWriter(FicConfig, False, System.Text.Encoding.Default)
            sw.WriteLine(If(optSQL.Checked, "1", "0"))
            sw.WriteLine(txtSqlDataSource.Text)
            sw.WriteLine(txtSqlInitialCatalog.Text)
            sw.WriteLine(If(chkSeguridadSQL.Checked, "1", "0"))
            sw.WriteLine(txtSqlUserId.Text)
            sw.WriteLine(txtSqlPassword.Text)
            sw.WriteLine(If(chkUsarDataAdapter.Checked, "1", "0"))
            sw.WriteLine(If(chkUsarCommandBuilder.Checked, "1", "0"))
            sw.WriteLine(If(chkUsarAddWithValue.Checked, "1", "0"))
            sw.WriteLine(If(chkUsarOverrides.Checked, "1", "0"))
            sw.WriteLine(If(optVB.Checked, "1", "0"))
            sw.WriteLine(If(chkPropiedadAuto.Checked, "1", "0"))
            sw.WriteLine(If(chkCrearIndizador.Checked, "1", "0"))
            ' Guardar el password de access                 (14/may/23 09.32)
            sw.WriteLine(txtAccessPassword.Text)
            ' También los otros 2 valores.                  (14/may/23 17.56)
            sw.WriteLine(txtAccessNombreBase.Text)
            sw.WriteLine(txtAccessProvider.Text)
        End Using
    End Sub

    ''' <summary>
    ''' Leer los datos de configuración, para no usar los settings
    ''' </summary>
    ''' <remarks>La comprobación de EndOfStream es por si no existía el fichero.</remarks>
    Private Sub LeerConfig()
        If File.Exists(FicConfig) Then
            Dim s As String
            Using sr = New StreamReader(FicConfig, System.Text.Encoding.Default, True)
                If sr.EndOfStream Then
                    s = "1"
                Else
                    s = sr.ReadLine()
                End If
                optSQL.Checked = s = "1"
                If sr.EndOfStream Then
                    s = ""
                Else
                    s = sr.ReadLine()
                End If
                txtSqlDataSource.Text = s
                If sr.EndOfStream Then
                    s = ""
                Else
                    s = sr.ReadLine()
                End If
                txtSqlInitialCatalog.Text = s
                If sr.EndOfStream Then
                    s = "0"
                Else
                    s = sr.ReadLine()
                End If
                chkSeguridadSQL.Checked = s = "1"
                If sr.EndOfStream Then
                    s = ""
                Else
                    s = sr.ReadLine()
                End If
                txtSqlUserId.Text = s
                If sr.EndOfStream Then
                    s = ""
                Else
                    s = sr.ReadLine()
                End If
                txtSqlPassword.Text = s
                If sr.EndOfStream Then
                    s = "0"
                Else
                    s = sr.ReadLine()
                End If
                chkUsarDataAdapter.Checked = s = "1"
                If sr.EndOfStream Then
                    s = "0"
                Else
                    s = sr.ReadLine()
                End If
                chkUsarCommandBuilder.Checked = s = "1"
                If sr.EndOfStream Then
                    s = "0"
                Else
                    s = sr.ReadLine()
                End If
                chkUsarAddWithValue.Checked = s = "1"
                If sr.EndOfStream Then
                    s = "1"
                Else
                    s = sr.ReadLine()
                End If
                chkUsarOverrides.Checked = s = "1"
                If sr.EndOfStream Then
                    s = "1"
                Else
                    s = sr.ReadLine()
                End If
                optVB.Checked = s = "1"
                If sr.EndOfStream Then
                    s = "1"
                Else
                    s = sr.ReadLine()
                End If
                chkPropiedadAuto.Checked = s = "1"
                If sr.EndOfStream Then
                    s = "1"
                Else
                    s = sr.ReadLine()
                End If
                chkCrearIndizador.Checked = s = "1"
                If sr.EndOfStream Then
                    s = ""
                Else
                    s = sr.ReadLine()
                End If
                txtAccessPassword.Text = s
                If sr.EndOfStream Then
                    s = ""
                Else
                    s = sr.ReadLine()
                End If
                txtAccessNombreBase.Text = s
                If sr.EndOfStream Then
                    s = ""
                Else
                    s = sr.ReadLine()
                End If
                txtAccessProvider.Text = s
            End Using
        Else
            txtSqlDataSource.Text = ""
            txtSqlInitialCatalog.Text = ""
            chkSeguridadSQL.Checked = True
            txtSqlUserId.Text = ""
            txtSqlPassword.Text = ""
            txtAccessNombreBase.Text = ""
            txtAccessProvider.Text = ""
            txtAccessPassword.Text = ""
            optSQL.Checked = True
            optVB.Checked = True
            chkUsarCommandBuilder.Checked = False
            chkUsarAddWithValue.Checked = False
            chkUsarDataAdapter.Checked = False
            chkUsarOverrides.Checked = True
            chkPropiedadAuto.Checked = True
            chkCrearIndizador.Checked = True
        End If
    End Sub

    Private Sub optVB_CheckedChanged(sender As Object, e As EventArgs) Handles optVB.CheckedChanged, optCS.CheckedChanged
        If optVB.Checked Then
            chkCrearIndizador.Text = "Crear Default Property"
        Else
            chkCrearIndizador.Text = "Crear indizador"
        End If
    End Sub
End Class
