Option Explicit On


Imports SpiderWebLibrary.graphElements
Imports SpiderWebLibrary.graphElements.compare
Imports SpiderWebLibrary.graphRepresentaions

Namespace graphTools

    ''' -------------------------------------------
    ''' Project : SpiderWebLibrary
    ''' Interface   : graphTree
    ''' 
    ''' <summary>
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' [Richard Schaffranek]   22/11/2013 created
    ''' </history>
    ''' 
    Public Interface graphTree

        ''' <summary>
        ''' Tests if the given Graph is a valid tree.
        ''' </summary>
        ''' <returns>Return true if the Graph is a tree, otherwise false</returns>
        ''' <remarks></remarks>
        ReadOnly Property isValidTree As Boolean

    End Interface
End Namespace