import jetbrains.buildServer.configs.kotlin.v2023_05.*
import jetbrains.buildServer.configs.kotlin.v2023_05.buildSteps.dotnet
import jetbrains.buildServer.configs.kotlin.v2023_05.triggers.vcs
import jetbrains.buildServer.configs.kotlin.v2023_05.vcs.GitVcsRoot

version = "2023.05"

project {

    vcsRoot(GitHubRepo)

    buildType(BuildAndTest)
}

object GitHubRepo : GitVcsRoot({
    name = "senior-engineer-home-exercise"
    url = "https://github.com/ace90210/senior-engineer-home-exercise.git"
    branch = "refs/heads/main"
    branchSpec = "+:refs/heads/*"
    authMethod = anonymous()
})

object BuildAndTest : BuildType({
    name = "Build & Test"

    vcs {
        root(GitHubRepo)
    }

    steps {
        dotnet {
            name = "Restore"
            command = dotnet.Command.restore
            projects = "src/People.sln"
        }

        dotnet {
            name = "Build"
            command = dotnet.Command.build
            projects = "src/People.sln"
            args = "--configuration Release"
        }

        dotnet {
            name = "Test"
            command = dotnet.Command.test
            projects = "src/People.sln"
            args = """
                --configuration Release 
                --logger trx 
                --results-directory test-results
            """.trimIndent()
        }
    }

    artifactRules = "test-results => test-results"

    features {
        feature {
            type = "xml-report-plugin"
            param("xmlReportParsing.reportType", "vstest")
            param("xmlReportParsing.reportDirs", "test-results/*.trx")
        }
    }

    triggers {
        vcs {
            branchFilter = "+:*"
        }
    }

    requirements {
        // Make sure agent has .NET 8 SDK installed
        exists("DotNetCoreSDK8.0")
    }
})
