Option Explicit On

Imports SpiderWebLibrary.graphElements
Imports SpiderWebLibrary.graphElements.compare


Namespace graphRepresentaions
    ''' -------------------------------------------
    ''' Project : SpiderWebLibrary
    ''' Class   : Graph
    ''' 
    ''' <summary>
    ''' Interface providing methodes across different graphRepresentations
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' [Richard Schaffranek]   22/11/2013 created
    ''' </history>
    ''' 

    Public Interface Graph

        ' ----------------- PROPERTIES --------------
        ''' <summary>
        ''' Cut a graph at the specified parameter.
        ''' </summary>
        ''' <param name="t">parameter to search for</param>
        ''' <param name="costs">cost at each graphVertex</param>
        ''' <value></value>
        ''' <returns>Returns two graphs. The first containing all parts of the graph with a value 
        ''' smaller and equals than the parameter t, another one with all parts larger and equals  than the parameter t
        ''' If the parameter t is on a graphEdge the graphEdge will be split.
        ''' </returns>
        ''' <remarks>s </remarks>
        ReadOnly Property split_tAt(ByVal t As Double, ByVal costs As List(Of Double)) As Graph()

        ''' <summary>
        ''' Cut a graph at the specified parameter.
        ''' </summary>
        ''' <param name="t">parameter to search for</param>
        ''' <param name="costs">cost at each graphVertex</param>
        ''' <value></value>
        ''' <returns>Returns a t value for each graphEdge within the given parameter. 
        ''' Use graphEdgeAt or graphEdgeIndexAt to get the coresponding graphEdge.
        ''' If costs of the graphEdge are specified, takes these costs into account and not only interpolates between the value of the connected graphVertices.
        ''' </returns>
        ''' <remarks>s </remarks>
        ReadOnly Property tAt(ByVal t As Double, ByVal costs As List(Of Double)) As List(Of Double)

        ''' <summary>
        ''' Find all graphEdges with a specified parameter.
        ''' </summary>
        ''' <param name="t">parameter to search for</param>
        ''' <param name="costs">cost at each graphVertex</param>
        ''' <value></value>
        ''' <returns>Returns a list of graphEdges with the given parameter.
        ''' </returns>
        ''' <remarks>s </remarks>
        ReadOnly Property graphEdge_tAt(ByVal t As Double, ByVal costs As List(Of Double)) As List(Of graphEdge)

        ''' <summary>
        ''' Find all graphEdges with a specified parameter.
        ''' </summary>
        ''' <param name="t">parameter to search for</param>
        ''' <param name="costs">cost at each graphVertex</param>
        ''' <value></value>
        ''' <returns>Returns the list indices of graphEdges with the given parameter.
        ''' </returns>
        ''' <remarks>s </remarks>
        ReadOnly Property graphEdgeIndex_tAt(ByVal t As Double, ByVal costs As List(Of Double)) As List(Of Integer)

        ''' <summary>
        ''' Checks if any edge of a Graph has a negative cost.
        ''' </summary>
        ''' <value></value>
        ''' <returns>Returns flase if no edge has costs smaler or equals to 0.</returns>
        ''' <remarks></remarks>
        ReadOnly Property negativeCost(Optional ByVal tol As Double = 1) As Boolean

        ''' <summary>
        ''' Counts the number of edges of a Graph
        ''' </summary>
        ''' <value></value>
        ''' <returns>Returns the number of edges within a Graph as Integer</returns>
        ''' <remarks>The highest index of an edge in the Graph is edgeCount-1</remarks>
        ReadOnly Property edgeCount As Integer

        ''' <summary>
        ''' Counts the number of vertices of a Graph
        ''' </summary>
        ''' <value></value>
        ''' <returns>Returns the number of vertices within a Graph as Integer</returns>
        ''' <remarks>The highest index of a vertex in the Graph is vertexCount-1</remarks>
        ReadOnly Property vertexCount As Integer

        ''' <summary>
        ''' Gets the graphEdges leading towards a specific vertex.
        ''' </summary>
        ''' <param name="i"></param>
        ''' <value></value>
        ''' <returns>Returns the inward edges as List(of graphEdge)</returns>
        ''' <remarks></remarks>
        ReadOnly Property getInEdges(ByVal i As Integer) As List(Of graphEdge)

        ''' <summary>
        ''' Gets the graphEdges leading away from a specific vertex.
        ''' </summary>
        ''' <param name="i"></param>
        ''' <value></value>
        ''' <returns>Returns the outward edges as List(of graphEdge)</returns>
        ''' <remarks></remarks>
        ReadOnly Property getOutEdges(ByVal i As Integer) As List(Of graphEdge)

        ''' <summary>
        ''' Gets the graphVertex that contain this graphVertex as neighbour.
        ''' </summary>
        ''' <param name="i"></param>
        ''' <value></value>
        ''' <returns>Returns the graphVertex  as List(of graphVertex)</returns>
        ''' <remarks></remarks>
        ReadOnly Property getInNB(ByVal i As Integer) As List(Of graphVertex)

        ''' <summary>
        ''' Gets the neighbouring graphVertx.
        ''' </summary>
        ''' <param name="i"></param>
        ''' <value></value>
        ''' <returns>Returns the graphVertex  as List(of graphVertex)</returns>
        ''' <remarks></remarks>
        ReadOnly Property getOutNB(ByVal i As Integer) As List(Of graphVertex)

        ''' <summary>
        ''' Gets the icidence representation of a Graph.
        ''' </summary>
        ''' <value></value>
        ''' <returns>Returns the Graph as List( of graphEdges)</returns>
        ''' <remarks></remarks>
        ReadOnly Property getEdgeList() As List(Of graphEdge)

        ''' <summary>
        ''' Gets the adjacency representation of a Graph.
        ''' </summary>
        ''' <value></value>
        ''' <returns>Returns the Graph as List( of graphEdges)</returns>
        ''' <remarks></remarks>
        ReadOnly Property getVertexList() As List(Of graphVertex)

        ' ----------------- SUB --------------
        ''' <summary>
        ''' Add a graphEdge into a Graph.
        ''' </summary>
        ''' <param name="gE"></param>
        ''' <remarks>This might break the order of elements within the Graph. If unsure use insert instate.</remarks>
        Sub add(ByVal gE As graphEdge)

        ''' <summary>
        ''' Add a graphVertex into a Graph.
        ''' </summary>
        ''' <param name="gV"></param>
        ''' <remarks>This might break the order of elements within the Graph. If unsure use insert instate.</remarks>
        Sub add(ByVal gV As graphVertex)

        ''' <summary>
        ''' Convert the Graph into its dualGraph. 
        ''' </summary>
        ''' <remarks>The cost of each Edge is set to 0</remarks>
        Sub dualGraph()

        ''' <summary>
        ''' Convert the Graph into its dualGraph and connects it to the vertices of the original Graph.
        ''' </summary>
        ''' <remarks>
        ''' The costs from the old vertices to the new vertices are either set to infinity (ingoing) or 0 (outgoing) all other edge costs are set to 0.
        ''' </remarks>
        Sub dualVertexGraph()

        ''' <summary>
        ''' Create a subGraph from a graph
        ''' </summary>
        ''' <param name="indices">List of indices to keep</param>
        ''' <remarks>Outcome depends on the Graph representation.</remarks>
        Sub subGraph(ByVal indices As List(Of Integer))

        ''' <summary>
        ''' Removes all elements of a Graph.
        ''' </summary>
        ''' <remarks></remarks>
        Sub clear()

        ''' <summary>
        ''' Ensures that the Graph is undirected.
        ''' </summary>
        ''' <remarks></remarks>
        Sub ensureUndirected()

        ''' <summary>
        ''' Union two Graphs.
        ''' </summary>
        ''' <param name="G"></param>
        ''' <returns>Returns the number of edges that could be added to this Graph.</returns>
        ''' <remarks></remarks>
        Function union(ByVal G As Graph) As Integer

        ' ----------------- Function --------------
        ''' <summary>
        ''' Insert a graphVertex into a Graph.
        ''' </summary>
        ''' <param name="gV"></param>
        ''' <returns>Returns the number of edges that could be added to the Grpah.</returns>
        ''' <remarks></remarks>
        Function insert(ByVal gV As graphVertex) As Integer

        ''' <summary>
        ''' Insert a graphEdge into a Graph.
        ''' </summary>
        ''' <param name="gE"></param>
        ''' <returns>Returns true if the graphEdge could be inserted, false otherwise</returns>
        ''' <remarks></remarks>
        Function insert(ByVal gE As graphEdge) As Boolean

        ''' <summary>
        ''' Remove a element from the Graph.
        ''' </summary>
        ''' <param name="index">Index of the element ot remove</param>
        ''' <returns>Returns true if succsessful.</returns>
        ''' <remarks>Outcome depends on the Graph representation.</remarks>
        Function delete(ByVal index As Integer) As Boolean

        ''' <summary>
        ''' Remove elements from the Graph.
        ''' </summary>
        ''' <param name="indices">List of indices to remove</param>
        ''' <returns>Returns the number of elements successfuly removed.</returns>
        ''' <remarks>Outcome depends on the Graph representation.</remarks>
        Function delete(ByVal indices As List(Of Integer)) As Integer

        ''' <summary>
        ''' Remove a graphEdge from the Graph.
        ''' </summary>
        ''' <param name="gE"></param>
        ''' <returns>Returns true if succsessful.</returns>
        ''' <remarks></remarks>
        Function delete(ByVal gE As graphEdge) As Boolean

        ''' <summary>
        ''' Removes the edges of a graphVertex from the Graph
        ''' </summary>
        ''' <param name="gV"></param>
        ''' <returns>Returns true if succsessful.</returns>
        ''' <remarks></remarks>
        Function delete(ByVal gV As graphVertex) As Integer

        ''' <summary>
        ''' Removes all graph vertices without in/out edges, and rempas their 
        ''' indices. e.g. Indices: 1, 2, 3, 5, 8, 9, 11 get remaped to 1, 2, 3, 4, 5, 6, 7
        ''' </summary>
        ''' <remarks></remarks>
        Function simplify() As List(Of Integer)

    End Interface
End Namespace