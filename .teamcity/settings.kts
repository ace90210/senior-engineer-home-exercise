import jetbrains.buildServer.configs.kotlin.v2019_2.*
import jetbrains.buildServer.configs.kotlin.v2019_2.buildFeatures.*
import jetbrains.buildServer.configs.kotlin.v2019_2.buildSteps.*
import jetbrains.buildServer.configs.kotlin.v2019_2.buildSteps.dotnet.*
import jetbrains.buildServer.configs.kotlin.v2019_2.vcs.*


// TeamCity Kotlin DSL version
version = "2020.1"

// Build settings
open class Settings {
    companion object {
        const val configuration = "Release"

        const val gitRepo = "https://github.com/ace90210/senior-engineer-home-exercise.git"
        const val gitBranch = "refs/heads/master"
        const val versionPrefix = "1.0.0"
        const val versionSuffix = "beta%build.number%"
    }
}

project {
    vcsRoot(GitHubRepo)
    buildType(BuildWebLinux64)
}

object GitHubRepo : GitVcsRoot({
    name = "senior-engineer-home-exercise"
    url = Settings.gitRepo
    branch = Settings.gitBranch
})

open class BuildBase(
    requiresSdk: Boolean
) : BuildType() {
    constructor(requiresSdk: Boolean, init: BuildBase.() -> Unit) : this(requiresSdk) {
        init()
    }

    init {
        vcs { root(GitHubRepo) }

        features {
            swabra {
                forceCleanCheckout = true
            }
        }

        if (requiresSdk) {
            requirements {
                exists("DotNetCoreSDK8.0_Path")
            }
        }
    }
}

open class BuildWebBase(
    runtimeId: String
) : BuildBase(true) {
    init {
        name = "Build console and web for $runtimeId"

        params {
            param("system.InvariantGlobalization", "true")
        }

      steps {
    step {
        type = "dotnet"
        param("command", "restore")
        param("projects", "src/People.sln")
    }

    step {
        type = "dotnet"
        param("command", "build")
        param("projects", "src/People.sln")
        param("args", "--configuration ${Settings.configuration}")
    }

    step {
        type = "dotnet"
        param("command", "test")
        param("projects", "src/People.sln")
        param("args", "--configuration ${Settings.configuration} --logger trx --results-directory test-results")
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
    }
}

// Actual build configuration
object BuildWebLinux64 : BuildWebBase("linux-x64")
