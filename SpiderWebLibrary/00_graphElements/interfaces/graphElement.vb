Option Explicit On


Imports SpiderWebLibrary.graphElements.compare

Namespace graphElements
    ''' -------------------------------------------
    ''' Project : SpiderWebLibrary
    ''' Interface : graphElement
    ''' 
    ''' <summary>
    ''' graphElement Interface implements some in common Properties and Functions of graphEdge and graphVertex.
    ''' </summary>
    ''' <history>
    ''' [Richard Schaffranek]   22/11/2013 created
    ''' </history>
    ''' 

    Public Interface graphElement

        ''' <summary>
        ''' Checks for negativeCosts.
        ''' </summary>
        ''' <value></value>
        ''' <returns>Return true if any connectin to the neighbours has negative costs, otherwise False.
        ''' </returns>
        ''' <remarks>
        ''' </remarks>
        ReadOnly Property negativeCost(Optional ByVal tol As Double = 1) As Boolean

        ''' <summary>
        ''' Get the maximum index.
        ''' </summary>
        ''' <value></value>
        ''' <returns>The maximum of index of this graphElement and its neighbourhood</returns>
        ''' <remarks></remarks>
        ReadOnly Property maxIndex As Integer

        ''' <summary>
        ''' Checks if a graphElement is valid. 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' Returns True if the vertex is valid, otherwise false.
        ''' </remarks>
        ReadOnly Property isValid As Boolean

        ''' <summary>
        ''' Remap vertex indices of graphElements
        ''' </summary>
        ''' <param name="map">List (of Integer)</param>
        ''' <remarks>Remaps the vertex indices to the position of the current vertex index within the map. If a index is not found in the map than it will be set to -1.</remarks>
        Sub remapVertex(ByVal map As List(Of Integer))

    End Interface
End Namespace