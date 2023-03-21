Option Explicit On


Imports SpiderWebLibrary.graphElements.compare

Namespace graphElements
    ''' -------------------------------------------
    ''' Project : SpiderWebLibrary
    ''' Class   : graphVertex
    ''' 
    ''' <summary>
    ''' graphVertex Class contains multible methodes to help the creation and manipulation of graphVertex
    ''' </summary>
    ''' <remarks>
    ''' Graphs represented through this class are limited to Int32.MaxValue vertices. This is due to it's simple GetHashCode() function.
    ''' HashCode = index of vertex 
    ''' </remarks>
    ''' <history>
    ''' [Richard Schaffranek]   22/11/2013 created
    ''' </history>
    ''' 

    Public Class graphVertex
        Implements gVComp, graphElement

        Private Const maxVertices As Integer = Int32.MaxValue
        Private vertexIndex As Integer

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks></remarks>
        Private nb As List(Of Integer)

        ''' <summary>
        ''' cost of the graphEdge connecting this graphVertex with the neighbour.
        ''' </summary>
        ''' <remarks>
        ''' If set to Double.PositiveInfinity, than the edge will be treated as having no costs.
        ''' This is different to a edgeCost of 0!
        ''' </remarks>
        Private nbCost As List(Of Double)

        ' -------------------- Constructor --------------------------

        ''' <overloads>
        ''' Constructor
        ''' </overloads>
        ''' <summary>
        ''' Creates a new  inValid instance of graphVertex
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        Public Sub New()
            Me.vertexIndex = -1
            Me.initialize()
        End Sub

        ''' <overloads>
        ''' Constructor
        ''' </overloads>
        ''' <summary>
        ''' Creates a new instance of graphVertex based on an existing graphVertex
        ''' </summary>
        ''' <param name="V">Existing graphVertex</param>
        ''' <remarks>
        ''' 
        ''' </remarks>
        Public Sub New(ByVal V As graphVertex)
            Me.vertexIndex = V.vertexIndex
            Me.initialize()
            Me.nb.AddRange(V.nb)
            Me.nbCost.AddRange(V.nbCost)
        End Sub

        ''' <overloads>
        ''' Constructor
        ''' </overloads>
        ''' <summary>
        ''' Creates a new instance of graphVertex with no neighbours
        ''' </summary>
        ''' <param name="i">Index of the new graphVertex</param>
        ''' <remarks>
        ''' 
        ''' </remarks>
        Public Sub New(ByVal i As Integer)
            vertexIndex = i
            Me.initialize()
        End Sub


        ''' <overloads>
        ''' Constructor
        ''' </overloads>
        ''' <summary>
        ''' Creates a new instance of graphVertex with neighbours.
        ''' </summary>
        ''' <param name="i">Index of the new graphVertex</param>
        ''' <param name="NB">List of graphVertex neighbours</param>
        ''' <remarks>
        ''' All edges with the neighbours have the cost of 0.
        ''' </remarks>
        Public Sub New(ByVal i As Integer, _
                    ByVal NB As List(Of Integer))
            vertexIndex = i
            Me.initialize()
            For j As Integer = 0 To NB.Count - 1
                Me.insert(NB.Item(j))
            Next
        End Sub

        ''' <overloads>
        ''' Constructor
        ''' </overloads>
        ''' <summary>
        ''' Creates a new instance of graphVertex with neighbours and costs.
        ''' </summary>
        ''' <param name="i">Index of the new graphVertex</param>
        ''' <param name="NB">List of graphVertex neighbours</param>
        ''' <param name="NB_Cost">List of costs traveling from graphVertex to its neighbours</param>
        ''' <remarks>
        ''' NB, NB_Cost must be of equal length.
        ''' </remarks>
        Public Sub New(ByVal i As Integer, _
                   ByVal NB As List(Of Integer), _
                   ByVal NB_Cost As List(Of Double))
            vertexIndex = i
            Me.initialize()
            For j As Integer = 0 To NB.Count - 1
                Me.insert(NB.Item(j), NB_Cost.Item(j))
            Next
        End Sub

        ''' <overloads>
        ''' Constructor
        ''' </overloads>
        ''' <summary>
        ''' Constructor a new grpahVertex form an existing graphEdge
        ''' </summary>
        ''' <param name="gE">graphEdge to construct the new graphVertex from.</param>
        ''' <remarks>
        ''' </remarks>
        Public Sub New(ByVal gE As graphEdge)
            Me.initialize()
            Me.vertexIndex = gE.A
            Me.nb.Add(gE.B)
            Me.nbCost.Add(gE.Cost)
        End Sub

        ' -------------------- graphElement --------------------------
        ''' <inheritdoc/>
        ReadOnly Property negativeCost(Optional ByVal tol As Double = 1) As Boolean Implements graphElement.negativeCost
            Get
                For Each c In Me.nbCost
                    If Math.Round(c / tol) < 0 Then
                        Return True
                    End If
                Next
                Return False
            End Get
        End Property

        ''' <inheritdoc/>
        ReadOnly Property maxIndex As Integer Implements graphElement.maxIndex
            Get
                If Me.outDegree > 0 Then
                    Return Math.Max(Me.index, Me.nb(Me.nb.Count - 1))
                End If
                Return Me.index
            End Get
        End Property

        ''' <inheritdoc/>
        ReadOnly Property isValid As Boolean Implements graphElement.isValid
            Get
                Return (Me.vertexIndex >= 0 And _
                Me.nbCost.Count = Me.outDegree And _
                Me.nb.Count = Me.outDegree And _
                Me.vertexIndex < maxVertices)
            End Get
        End Property

        ''' <inheritdoc/>
        Protected Friend Sub remapVertex(ByVal map As List(Of Integer)) Implements graphElement.remapVertex
            Me.vertexIndex = map.BinarySearch(Me.index)
            For i As Integer = 0 To Me.outDegree - 1
                Me.nb(i) = map.BinarySearch(Me.nb(i))
            Next
        End Sub

        ' -------------------- Property --------------------------
        ''' <summary>
        ''' index of node
        ''' </summary>
        ''' <value>Must be larger than or equal to 0</value>
        ''' <returns>Index of graphVertex</returns>
        ''' <remarks> </remarks>
        Property index() As Integer
            Get
                Return vertexIndex
            End Get
            Set(value As Integer)
                If value >= 0 Then
                    vertexIndex = value
                End If
            End Set
        End Property

        ''' <summary>
        ''' Neighbours of graphVertex
        ''' </summary>
        ''' <value></value>
        ''' <returns>List(of Integer)</returns>
        ''' <remarks> </remarks>
        ReadOnly Property neighbours As Integer()
            Get
                Return Me.nb.ToArray()
            End Get
        End Property

        ''' <summary>
        ''' Neighbours of graphVertex
        ''' </summary>
        ''' <param name="i">index of the neighbour</param>
        ''' <value></value>
        ''' <returns>returns the neighbour at the index(i)</returns>
        ''' <remarks></remarks>
        ReadOnly Property neighbours(ByVal i As Integer) As Integer
            Get
                If Me.nb.Count > i Then
                    Return Me.nb(i)
                Else
                    Return -1
                End If
            End Get
        End Property


        ''' <summary>
        ''' Costs of the edges connecting to the grpahVertex neighbours.
        ''' </summary>
        ''' <value></value>
        ''' <returns>List(of Double). Returned List is of the same length as neighbours()</returns>
        ''' <remarks> </remarks>
        ReadOnly Property cost As Double()
            Get
                Return Me.nbCost.ToArray()

            End Get
        End Property

        ''' <summary>
        ''' Costs of the edges connecting to the grpahVertex neighbours.
        ''' </summary>
        ''' <param name="i">index of the neighbour</param>
        ''' <value></value>
        ''' <returns>Returns the cost to go from this graphVertex to the neighbour with at index(i)</returns>
        ''' <remarks></remarks>
        Public Property cost(ByVal i As Integer) As Double
            Get
                If Me.nbCost.Count > i Then
                    Return Me.nbCost(i)
                Else
                    Return -1
                End If
            End Get
            Set(value As Double)
                If Me.nbCost.Count > i Then
                    Me.nbCost(i) = value
                End If
            End Set
        End Property


        ''' <summary>
        ''' Collects all outgoing Edges
        ''' </summary>
        ''' <value></value>
        ''' <returns>Returns all outgoing Edges as graphEdge
        ''' </returns>
        ''' <remarks>
        ''' </remarks>
        ReadOnly Property outEdges() As List(Of graphEdge)
            Get
                Dim gEL As New List(Of graphEdge)
                Dim gE As graphEdge
                Dim cursor As Integer
                For i As Integer = 0 To Me.outDegree - 1
                    gE = New graphEdge(vertexIndex, Me.nb(i), Me.nbCost(i))
                    cursor = gEL.BinarySearch(gE, gE)
                    If cursor < 0 Then
                        cursor = cursor Xor -1
                        gEL.Insert(cursor, gE)
                    End If
                Next
                Return gEL
            End Get
        End Property ' Computes in O(n), where n is the outDegree of the vertex.

        ''' <summary>
        ''' outDegree of the graphVertex
        ''' </summary>
        ''' <value></value>
        ''' <returns>as Integer </returns>
        ''' <remarks></remarks>
        ReadOnly Property outDegree As Integer
            Get
                Return Me.nb.Count
            End Get
        End Property

        ' -------------------- Overrides --------------------------

        ''' <summary>
        ''' Returns the String representation.
        ''' </summary>
        ''' <returns>String</returns>
        ''' <remarks>
        ''' index: (i), outDegree: (d)
        ''' </remarks>
        Public Overrides Function ToString() As String
            Dim str As String = "index: " & vertexIndex & " [ "
            For Each item In Me.nb
                str = str & item & " "
            Next
            Return str & "]" & ", outDegree: " & Me.outDegree
        End Function


        ''' <summary>
        ''' Tests for equality. Uses HashCode as basis.
        ''' </summary>
        ''' <param name="obj"></param>
        ''' <returns>Return true if equal, otherwise false or if inValid false.</returns>
        ''' <remarks></remarks>
        Public Overrides Function Equals(obj As Object) As Boolean
            If obj Is Nothing OrElse Not [GetType]().Equals(obj.GetType()) Then
                Return False
            End If
            Dim V As graphVertex = CType(obj, graphVertex)
            If V.isValid And Me.isValid And V.GetHashCode() = Me.GetHashCode() Then
                Return True
            End If
            Return False
        End Function

        ''' <summary>
        ''' Hashfunction of the graphVertex.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>HashCode = index. This Limits the amount of graphVertices that can be compared Int32.MaxValue.
        ''' If the graphVertex is not Valid this will return -1</remarks>
        Public Overrides Function GetHashCode() As Integer
            If Not isValid() Then
                Return -1
            End If
            Return vertexIndex
        End Function

        ' -------------------- IComparer --------------------------
        ''' <summary>
        ''' Compare two graphVertices
        ''' </summary>
        ''' <param name="V1"></param>
        ''' <param name="V2"></param>
        ''' <returns>Returns 1 if HashCode of V1 larger HashCode of V2, 0 if equal, -1 if  HashCode of V1 smaller HashCode of V2</returns>
        ''' <remarks>
        ''' Sorts graphEdges with asscenting HashCode.
        ''' </remarks>
        Public Function Compare(ByVal V1 As graphVertex, ByVal V2 As graphVertex) As Integer Implements IComparer(Of graphVertex).Compare
            If Not V1.isValid Then
                If Not V2.isValid Then
                    Return 0
                Else
                    Return -1
                End If
            Else
                If Not V2.isValid Then
                    Return 1
                Else
                    If V1.index > V2.index Then
                        Return 1
                    ElseIf V2.index > V1.index Then
                        Return -1
                    Else
                        Return 0
                    End If
                End If
            End If
        End Function

        ' -------------------- Public -------------------------- 
        ''' <summary>
        ''' Searches for the Neighbour of a graphVertex.
        ''' </summary>
        ''' <param name="nbIndex"></param>
        ''' <returns>Returns The zero-based index of item, if item is found; 
        ''' otherwise, a negative number that is the bitwise complement of the index of the next element 
        ''' that is larger than item or, if there is no larger element, the bitwise complement of Count.
        ''' </returns>
        ''' <remarks>Computes in O(log n), where n is the number of neighbours.</remarks>
        Public Function findNB(ByVal nbIndex As Integer) As Integer
            Return Me.nb.BinarySearch(nbIndex)
        End Function

        ''' <summary>
        ''' Removes a neighbour.
        ''' </summary>
        ''' <param name="nbIndex"></param>
        ''' <returns>Returns true if successful, flase otherwise</returns>
        ''' <remarks>Computes in O(log n), where n is the number of neighbours.</remarks>
        Public Function removeNB(ByVal nbIndex As Integer) As Boolean
            Dim cursor As Integer = Me.findNB(nbIndex)
            If cursor >= 0 Then
                Me.nb.RemoveAt(cursor)
                Me.nbCost.RemoveAt(cursor)
                Return True
            End If
            Return False
        End Function

        ''' <summary>
        ''' Inserts a graphEdge into the neighbourhood of a graphVertex.
        ''' </summary>
        ''' <param name="gE"></param>
        ''' <returns>Returns True if successful, false otherwise.</returns>
        ''' <remarks>Computes in O(log n), where n is the number of neighbours.</remarks>
        Public Function insert(ByVal gE As graphEdge) As Boolean
            If Not gE.isValid Then Return False
            Dim cursor As Integer
            If vertexIndex = gE.A Then
                cursor = Me.findNB(gE.B)
                If cursor < 0 Then
                    cursor = cursor Xor -1
                    Me.nb.Insert(cursor, gE.B)
                    Me.nbCost.Insert(cursor, gE.Cost)
                    Return True
                End If
            End If
            Return False
        End Function

        ''' <summary>
        ''' Inserts a new neighbour into the graphVertex neighbourhood. 
        ''' </summary>
        ''' <param name="nbI"></param>
        ''' <param name="nbC"></param>
        ''' <returns>Returns True if successful, false otherwise.</returns>
        ''' <remarks>Computes in O(log n), where n is the number of neighbours.</remarks>
        Public Function insert(ByVal nbI As Integer, _
                                 Optional ByVal nbC As Double = Double.PositiveInfinity) As Boolean
            If nbI < 0 Then Return False

            Dim cursor As Integer
            cursor = Me.findNB(nbI)
            If cursor < 0 Then
                cursor = cursor Xor -1
                Me.nb.Insert(cursor, nbI)
                Me.nbCost.Insert(cursor, nbC)
                Return True
            End If

            Return False
        End Function

        ''' <summary>
        ''' Merges a graphVertex with this graphVertex
        ''' </summary>
        ''' <param name="gV"></param>
        ''' <returns></returns>
        ''' <remarks>Computes in O(m log n), where n is the number of neighbours of this graphVertex and m is the number of neighbours of the grpahVertex to merge.</remarks>
        Public Function merge(ByVal gV As graphVertex) As Integer
            Dim ret As Integer = 0
            If gV.isValid() And Me.vertexIndex = gV.vertexIndex Then
                For vI As Integer = 0 To gV.outDegree - 1
                    ret += Convert.ToInt32(Me.insert(gV.nb(vI), gV.nbCost(vI)))
                Next
            End If
            Return ret
        End Function

        'tested
        ''' <summary>
        ''' Removes all neighbours from this graphVertex 
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub initialize()
            Me.nb = New List(Of Integer)
            Me.nbCost = New List(Of Double)
        End Sub

        ' tested
        ''' <summary>
        ''' Median value of the neighbourhood.
        ''' </summary>
        ''' <param name="Values">The list has to be as long as the maxIndex of this gaphVertex or the neighborhood.</param>
        ''' <returns>Returns the median value as double of the neighbourhood specified by the graphVertex.
        ''' If the list length is smaller than the maxIndex NaN will be returned.
        ''' </returns>
        ''' <remarks>Computes in O(n log n), where n is the number of neighbours.</remarks>
        Protected Friend Function median(ByVal Values As List(Of Double)) As Double
            If Me.maxIndex >= Values.Count Then
                Return Double.NaN
            End If

            Dim tmpValues As New List(Of Double)
            Dim cursor As Integer
            tmpValues.Add(Values.Item(Me.index))

            For Each i As Integer In Me.nb
                tmpValues.Add(Values.Item(i))
            Next

            tmpValues.Sort()
            cursor = (Math.Max(tmpValues.Count - 1, 0) / 2)

            Return tmpValues.Item(cursor)
        End Function

        'tested
        ''' <summary>
        ''' Average value of the neighbourhood.
        ''' </summary>
        ''' <param name="Values">The list has to be as long as the highest index of the vertex or the neighborhood.</param>
        ''' <returns>Returns the average value as double of the neighbourhood specified by the graphVertex.
        ''' If the list length is smaller than the maxIndex NaN will be returned.
        ''' </returns>
        ''' <remarks>Computes in O(n), where n is the number of neighbours.</remarks>
        Protected Friend Function average(ByVal Values As List(Of Double)) As Double
            If Me.maxIndex >= Values.Count Then
                Return Double.NaN
            End If

            Dim sum As Double = Values.Item(Me.index)

            For Each i As Integer In Me.nb
                sum += Values.Item(i)
            Next

            Return sum / (1 + Me.outDegree)
        End Function

        ''' <summary>
        ''' Checks if the graphVertex is a Point of Intrest within the neighbourhood.
        ''' </summary>
        ''' <param name="Values">The list has to be as long as the highest index of the vertex or the neighborhood.</param>
        ''' <returns>Returns True if all other Values within the neighbourhood are larger or equal to the value of this graphVertex.
        '''  If the list length is smaller than the maxIndex NaN will be returned.
        ''' </returns>
        ''' <remarks>Computes in O(n), where n is the number of neighbours.</remarks>
        Protected Friend Function isPOI(ByVal Values As List(Of Double)) As Boolean
            If Me.maxIndex >= Values.Count Then
                Return Double.NaN
            End If

            Dim currValue As Double = Values.Item(Me.index)

            For Each i As Integer In Me.nb
                If Values.Item(i) <= currValue Then
                    Return False
                End If
            Next

            Return True
        End Function


    End Class

End Namespace
