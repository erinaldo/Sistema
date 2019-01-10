﻿Imports MySql.Data.MySqlClient

Public Class TipoProdDAO
    Public Function getTipos() As DataSet
        Dim clientes As New Collection
        Dim ds As New DataSet
        Dim da As New MySqlDataAdapter
        Dim dt As New DataTable
        Try
            Dim con As New MySqlConnection(ConexionDB.cadenaConexionBD(Sesion.Usuario, Sesion.Password))
            con.Open()


            Dim mysql = "SELECT * from vlistadotipoprod"

            Dim cmd As New MySqlCommand(mysql, con)
            Dim adp As New MySqlDataAdapter(mysql, con)

            ds.Tables.Add("tabla")
            adp.Fill(ds.Tables("tabla"))

            con.Close()
        Catch ex As Exception
            Throw New DAOException(ex.ToString)
        End Try
        Return ds
    End Function

    Public Sub agregar(ByVal modelo As TipoProducto)
        Try
            Dim con As New MySqlConnection(ConexionDB.cadenaConexionBD(Sesion.Usuario, Sesion.Password))
            con.Open()

            Dim query As String = "INSERT INTO `prodtipo` (`prodTipo`) VALUES (@tipo);"
            Dim cmd As New MySqlCommand(query, con)

            cmd.Parameters.AddWithValue("@tipo", modelo.nombre)


            cmd.ExecuteNonQuery()
            con.Close()
        Catch ex As Exception
            Throw New DAOException(ex.ToString)
        End Try
    End Sub

    Public Sub eliminar(ByVal id As String)
        Try
            Dim con As New MySqlConnection(ConexionDB.cadenaConexionBD(Sesion.Usuario, Sesion.Password))
            con.Open()

            Dim query As String = "DELETE FROM `prodtipo` WHERE `prodTipocod` = @id;"
            Dim cmd As New MySqlCommand(query, con)

            cmd.Parameters.AddWithValue("@id", id)


            cmd.ExecuteNonQuery()
            con.Close()
        Catch ex As Exception
            Throw New DAOException(ex.ToString)
        End Try
    End Sub
End Class