import jetbrains.buildServer.configs.kotlin.v2025_07.*
import jetbrains.buildServer.configs.kotlin.v2025_07.Project
import jetbrains.buildServer.configs.kotlin.v2025_07.buildSteps.script



version = "2025.07"


object BuildAndPushImage : BuildType({
    name = "Build, Test, and Push Docker Image"

    buildNumberPattern = "%build.counter%"

    params {
        param("docker.registry", "localhost:5000")
        param("image.name", "people-api")
    }

    vcs {
        root(DslContext.settingsRoot)
        cleanCheckout = true
    }

    steps {
        script {
            name = "Restore .NET Dependencies"
            scriptContent = "dotnet restore src/People.sln"
        }
        script {
            name = "Build Project"
            scriptContent = "dotnet build src/People.sln --no-restore"
        }
        script {
            name = "Run Unit Tests"
            scriptContent = "dotnet test src/People.sln --no-build --logger trx"
        }
        script {
            name = "Build Docker Image"
            scriptContent = "docker build -t %docker.registry%/%image.name%:%build.number% ."
        }
        script {
            name = "Push Docker Image to Local Registry"
            scriptContent = "docker push %docker.registry%/%image.name%:%build.number%"
        }
        script {
            name = "Save Image Digest as Artifact"
            scriptContent = "docker inspect --format='{{index .RepoDigests 0}}' %docker.registry%/%image.name%:%build.number% > image.digest"
        }
    }

    artifacts {
        artifactRules = "image.digest"
    }

    requirements {
        exists("docker.server.version")
    }
})

project {
    buildType(BuildAndPushImage)
}