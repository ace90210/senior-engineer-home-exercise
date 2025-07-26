import jetbrains.buildServer.configs.kotlin.v2019_2.*
import jetbrains.buildServer.configs.kotlin.v2019_2.buildFeatures.*
import jetbrains.buildServer.configs.kotlin.v2019_2.buildSteps.*
import jetbrains.buildServer.configs.kotlin.v2019_2.vcs.*
import java.util.*

// TeamCity Kotlin DSL version
version = "2020.1"

// Build settings
open class Settings {
    companion object {
        const val configuration = "Release"

        const val gitRepo = "https://github.com/ace90210/senior-engineer-home-exercise.git"
        const val gitBranch = "refs/heads/master"

        // You can use versionPrefix to set the "base" version number for your library/app. https://andrewlock.net/version-vs-versionsuffix-vs-packageversion-what-do-they-all-mean/#versionprefix
        const val versionPrefix = "1.0.0"
        // Is used to set the pre-release label of the version number, if there is one, such as alpha or beta. https://andrewlock.net/version-vs-versionsuffix-vs-packageversion-what-do-they-all-mean/#versionsuffix
        const val versionSuffix = "beta%build.number%"

    }
}

project {

    vcsRoot(GitHubRepo)

    buildType(BuildAndTest)
}


object BuildingProject : Project({
    name = "Building"
    buildType(BuildWebLinux64)
})

object GitHubRepo : GitVcsRoot({
    name = "senior-engineer-home-exercise"
    url =  Settings.gitRepo
    branch = Settings.gitBranch
})


// Base configuration for builds and tests
open class BuildBase(
        // True to add agents requirements
        requiresSdk: Boolean)
    : BuildType() {
    constructor(requiresSdk: Boolean, init: BuildBase.() -> Unit): this(requiresSdk) {
        init()
    }
    init {
        vcs { root(GitHubRepo) }
        features {
            // Clear the checkout directory before building
            swabra {
                forceCleanCheckout = true
            }
        }
        if (requiresSdk) {
            // Agents requirement to have .NET Core SDK 8.0
            requirements {
                exists("DotNetCoreSDK8.0_Path")
            }
        }
    }
}


// Base configuration to build cross-platform applications
open class BuildWebBase(
        runtimeId: String)
    : BuildBase(true) {
    init {
        name = "Build console and web for $runtimeId"
       
        params {
               param("system.InvariantGlobalization", "true")
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
        // Publish the content of bin directory as build artifacts
        artifactRules = "bin => bin"
    }
}

object BuildWebLinux64: BuildWebBase("linux-x64")